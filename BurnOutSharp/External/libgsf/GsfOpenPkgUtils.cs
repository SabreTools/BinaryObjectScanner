/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-open-pkg-utils.c: Utilities for handling Open Package zip files
 * 			from MS Office 2007 or XPS.
 *
 * Copyright (C) 2006-2007 Jody Goldberg (jody@gnome.org)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using LibGSF.Input;
using LibGSF.Output;

namespace LibGSF
{
    public class GsfOpenPkgRel
    {
        #region Properties

        public string Id { get; set; } = null;

        public string Type { get; set; } = null;

        public string Target { get; set; } = null;

        public bool IsExtern { get; set; } = false;

        #endregion

        #region Functions

        /// <returns>A new GsfInput which the called needs to unref, or null and sets <paramref name="err"/></returns>
        public static GsfInput OpenRelatedPackage(GsfInput opkg, GsfOpenPkgRel rel, ref Exception err)
        {
            if (opkg == null)
                return null;
            if (rel == null)
                return null;

            GsfInput res = null;
            GsfInfile prev_parent;

            // References from the root use children of opkg
            // References from a child are relative to siblings of opkg
            GsfInfile parent = opkg.Name != null ? opkg.Container : opkg as GsfInfile;

            string target = rel.Target;
            if (target[0] == '/')
            {
                target = target.Substring(1);

                // Handle absolute references by going as far up as we can.
                while (true)
                {
                    GsfInfile next_parent = parent.Container;
                    if (next_parent != null && next_parent.GetType() == parent.GetType())
                        parent = next_parent;
                    else
                        break;
                }
            }

            string[] elems = rel.Target.Split('/');
            for (int i = 0; i < elems.Length && elems[i] != null && parent != null; i++)
            {
                if (elems[i] == "." || elems[i][0] == '\0')
                    continue; // Ignore '.' and empty

                prev_parent = parent;
                if (elems[i] == "..")
                {
                    parent = parent.Container;
                    res = null; // Only return newly created children
                    if (parent != null)
                    {
                        // Check for attempt to gain access outside the zip file
                        if (parent.GetType() != prev_parent.GetType())
                        {
                            Console.Error.WriteLine("Broken file: relation access outside container");
                            parent = null;
                        }
                    }
                }
                else
                {
                    res = parent.ChildByName(elems[i], ref err);
                    if (elems[i + 1] != null)
                    {
                        if (res == null || !(res is GsfInfile))
                            return null;

                        parent = res as GsfInfile;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Finds <paramref name="opkg"/>'s relation with @id
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <returns>A GsfOpenPkgRel or null</returns>
        /// <remarks>
        /// New in 1.14.6
        /// 
        /// Skipping because gsf_open_pkg_rel_get_type() does not return a GType.
        /// </remarks>
        public static GsfOpenPkgRel LookupRelationById(GsfInput opkg, string id)
        {
            GsfOpenPkgRels rels = GsfOpenPkgRels.LookupRelations(opkg);
            if (rels == null)
                return null;

            if (!rels.RelationsById.ContainsKey(id))
                return null;

            return rels.RelationsById[id];
        }

        /// <summary>
        /// Finds _a_ relation of <paramref name="opkg"/> with <paramref name="type"/> (no order is guaranteed)
        /// </summary>
        /// <param name="type">Target</param>
        /// <returns>A GsfOpenPkgRel or null</returns>
        /// <remarks>
        /// New in 1.14.6
        /// 
        /// Skipping because gsf_open_pkg_rel_get_type() does not return a GType.
        /// </remarks>
        public static GsfOpenPkgRel LookupRelationByType(GsfInput opkg, string type)
        {
            GsfOpenPkgRels rels = GsfOpenPkgRels.LookupRelations(opkg);
            if (rels == null)
                return null;

            if (!rels.RelationsByType.ContainsKey(type))
                return null;

            return rels.RelationsByType[type];
        }

        #endregion
    }

    public class GsfOpenPkgRels
    {
        #region Constants

        /// <summary>
        /// Generated based on:
        /// http://www.oasis-open.org/committees/download.php/12572/OpenDocument-v1.0-os.pdf
        /// and  OpenDocument-v1.1.pdf
        /// </summary>
        private static XmlNameTable CREATE_NAMESPACES()
        {
            NameTable table = new NameTable();

            table.Add("http://schemas.openxmlformats.org/package/2006/relationships");

            return table;
        }

        private static XmlDocumentType CREATE_DTD(XmlDocument doc)
        {
            // Root node
            XmlDocumentType docType = doc.CreateDocumentType(null, null, null, null);

            docType.AppendChild(doc.CreateElement("Relationships"));
            docType["Relationships"].AppendChild(doc.CreateElement("Relationship"));

            return docType;
        }

        #endregion

        #region Properties

        public Dictionary<string, GsfOpenPkgRel> RelationsById { get; set; } = new Dictionary<string, GsfOpenPkgRel>();

        public Dictionary<string, GsfOpenPkgRel> RelationsByType { get; set; } = new Dictionary<string, GsfOpenPkgRel>();

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOpenPkgRels() { }

        #endregion

        #region Functions

        public static GsfOpenPkgRels LookupRelations(GsfInput opkg)
        {
            if (opkg == null)
                return null;

            GsfOpenPkgRels rels = opkg.Relations;
            if (rels == null)
            {
                string part_name = opkg.Name;

                GsfInput rel_stream;
                if (part_name != null)
                {
                    GsfInfile container = opkg.Container;
                    if (container == null)
                        return null;

                    string rel_name = $"{part_name}.rels";
                    rel_stream = container.ChildByVariableName("_rels", rel_name);
                }
                else // The root
                {
                    rel_stream = (opkg as GsfInfile).ChildByVariableName("_rels", ".rels");
                }

                XmlDocument rel_doc;
                if (rel_stream != null)
                {
                    rels = new GsfOpenPkgRels
                    {
                        RelationsById = new Dictionary<string, GsfOpenPkgRel>(),
                        RelationsByType = new Dictionary<string, GsfOpenPkgRel>(),
                    };

                    rel_doc = new XmlDocument(CREATE_NAMESPACES());
                    XmlDocumentType docType = CREATE_DTD(rel_doc);
                    rel_doc.AppendChild(docType);

                    // TODO: Enable parsing
                    //rel_doc.Parse(rel_stream, rels);
                }

                opkg.Relations = rels;
            }

            return rels;
        }

        #endregion

        #region Utilities

        private static void OpenPkgRelBegin(GsfXMLIn xin, string[] attrs)
        {
            GsfOpenPkgRels rels = xin.UserState as GsfOpenPkgRels;
            string id = null;
            string type = null;
            string target = null;
            bool is_extern = false;

            for (int i = 0; i < attrs.Length && attrs[i] != null && attrs[i + 1] != null; i += 2)
            {
                switch (attrs[i])
                {
                    case "Id":
                        id = attrs[i + 1];
                        break;
                    case "Type":
                        type = attrs[i + 1];
                        break;
                    case "Target":
                        target = attrs[i + 1];
                        break;
                    case "TargetMode":
                        is_extern = attrs[i + 1] == "External";
                        break;
                }
            }

            if (id == null)
            {
                Console.Error.WriteLine("Broken relation: missing id");
                id = "?";
            }

            if (type == null)
            {
                Console.Error.WriteLine("Broken relation: missing type");
                type = "?";
            }

            if (target == null)
            {
                Console.Error.WriteLine("Broken relation: missing target");
                target = "?";
            }

            GsfOpenPkgRel rel = new GsfOpenPkgRel
            {
                Id = id,
                Type = type,
                Target = target,
                IsExtern = is_extern,
            };

            // Make sure we don't point to a freed rel in the type hash.
            rels.RelationsByType[rel.Type] = rel;

            // This will free a duplicate rel, so do this last.
            rels.RelationsById[rel.Id] = rel;
        }

        #endregion
    };

    public class GsfOutfileOpenPkg : GsfOutfile
    {
        #region Properties

        public GsfOutput Sink { get; set; } = null;

        public bool IsDir { get; set; } = false;

        public string ContentType { get; set; } = null;

        public List<GsfOutfileOpenPkg> Children { get; set; } = null;

        public List<GsfOpenPkgRel> Relations { get; set; } = null;

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutfileOpenPkg() { }

        /// <summary>
        /// Convenience routine to create a GsfOutfileOpenPkg inside <paramref name="sink"/>.
        /// </summary>
        /// <param name="sink"></param>
        /// <returns>A GsfOutfile that the caller is responsible for.</returns>
        public static GsfOutfileOpenPkg Create(GsfOutfile sink)
        {
            return new GsfOutfileOpenPkg
            {
                Sink = sink,
                IsDir = true,
            };
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfOutfileOpenPkg()
        {
            if (Sink != null)
                Sink = null;

            ContentType = null;

            Children.Clear();
            Children = null;
        }

        #endregion

        #region Functions

        public new bool Close()
        {
            GsfOutput dir;
            bool res = false;
            string rels_name;

            if (Sink == null || Sink.IsClosed)
                return true;

            // Generate [Content_types].xml when we close the root dir
            if (Name == null)
            {
                GsfOutput output = (Sink as GsfOutfile).NewChild("[Content_Types].xml", false);
                GsfXMLOut xml = GsfXMLOut.Create(output);

                xml.StartElement("Types");
                xml.AddStringUnchecked("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");
                xml.WriteContentDefault("rels", "application/vnd.openxmlformats-package.relationships+xml");
                xml.WriteContentDefault("xlbin", "application/vnd.openxmlformats-officedocument.spreadsheetml.printerSettings");
                xml.WriteContentDefault("xml", "application/xml");
                xml.WriteContentDefault("vml", "application/vnd.openxmlformats-officedocument.vmlDrawing");
                xml.WriteContentOverride(this, "/");
                xml.EndElement();

                output.Close();

                dir = Sink;
                rels_name = ".rels";
            }
            else
            {
                res = Sink.Close();
                dir = Sink.Container;
                rels_name = $"{Name}.rels";
            }

            if (Relations != null)
            {
                dir = (dir as GsfOutfile).NewChild("_rels", true);
                GsfOutput rels = (dir as GsfOutfile).NewChild(rels_name, false);
                GsfXMLOut xml = GsfXMLOut.Create(rels);

                xml.StartElement("Relationships");
                xml.AddStringUnchecked("xmlns", "http://schemas.openxmlformats.org/package/2006/relationships");

                foreach (GsfOpenPkgRel rel in Relations)
                {
                    xml.StartElement("Relationship");
                    xml.AddString("Id", rel.Id);
                    xml.AddString("Type", rel.Type);
                    xml.AddString("Target", rel.Target);
                    if (rel.IsExtern)
                        xml.AddStringUnchecked("TargetMode", "External");

                    xml.EndElement();
                }

                Relations.Clear();
                xml.EndElement();
                rels.Close();
            }

            // Close the container
            if (Name == null)
                return Sink.Close();

            return res;
        }

        public string CreateRelation(string target, string type, bool is_extern)
        {
            GsfOpenPkgRel rel = new GsfOpenPkgRel
            {
                Target = target,
                Type = type,
                Id = $"rID{Relations.Count + 1}",
                IsExtern = is_extern,
            };

            Relations.Add(rel);
            return rel.Id;
        }

        /// <summary>
        /// Create a relationship between child and <paramref name="parent"/> of <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Target type</param>
        /// <returns>The relID which the caller does not own but will live as long as <paramref name="parent"/>.</returns>
        public string RelatePackages(GsfOutfileOpenPkg parent, string type)
        {
            int up = -1;
            GsfOutfile child_dir;

            // Calculate the path from child to parent
            GsfOutfile parent_dir = parent.IsDir ? parent : parent.Container;
            do
            {
                up++;
                child_dir = this;
                while ((child_dir = child_dir.Container) != null)
                {
                    if (child_dir == parent_dir)
                        goto found; // Break out of both loops
                }
            } while ((parent_dir = parent_dir.Container) != null);

            // TODO: Figure out how to best remove this goto
        found:
            // Yes prepend is slow, this will never be preformance critical
            string path = Name;
            child_dir = this;
            while ((child_dir = child_dir.Container) != null && child_dir.Name != null && child_dir != parent_dir)
            {
                path = $"{child_dir.Name}/{path}";
            }

            while (up-- != 0)
            {
                path = $"../{path}";
            }

            return parent.CreateRelation(path, type, false);
        }

        /// <summary>
        /// A convenience wrapper to create a child in <paramref name="dir"/> of <paramref name="content_type"/> then create
        /// a <paramref name="type"/> relation to <paramref name="parent"/>
        /// </summary>
        /// <param name="name">Target name</param>
        /// <param name="content_type">Non-null content type</param>
        /// <param name="type">Target type</param>
        /// <returns>The new part.</returns>
        public GsfOutput AddRelation(string name, string content_type, GsfOutfile parent, string type)
        {
            GsfOutfileOpenPkg part = NewChild(name, false) as GsfOutfileOpenPkg;
            if (part == null)
                return null;

            part.ContentType = content_type;
            part.RelatePackages(parent as GsfOutfileOpenPkg, type);
            return part;
        }

        /// <summary>
        /// Add an external relation to parent.
        /// </summary>
        /// <param name="target">Target type</param>
        /// <param name="content_type">Target content</param>
        /// <returns>
        /// The id of the relation.  The string is
        /// managed by the parent and should not be changed or freed by the
        /// caller.
        /// </returns>
        public string AddExternalRelation(string target, string content_type) => CreateRelation(target, content_type, true);

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data) => Sink.Write(num_bytes, data);

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence) => Sink.Seek(offset, whence);

        /// <inheritdoc/>
        public override GsfOutput NewChild(string name, bool is_dir)
        {
            if (!is_dir)
                return null;

            GsfOutfileOpenPkg child = new GsfOutfileOpenPkg
            {
                Name = name,
                Container = this,
                IsDir = is_dir,
            };

            // Holding a ref here is not ideal.  It means we won't release any of the
            // children until the package is closed.
            Children.Add(child);

            return child;
        }

        #endregion
    }

    public static class GsfOpenPkgUtils
    {
        #region Delegates

        public delegate void GsfOpenPkgIter(GsfInput opkg, GsfOpenPkgRel rel, object user_data);

        #endregion

        #region Classes

        private class pkg_iter_data
        {
            public GsfInput opkg { get; set; }

            public GsfOpenPkgIter func { get; set; }

            public object user_data { get; set; }
        };

        #endregion

        #region Functions

        /// <summary>
        /// Walks each relationship associated with <paramref name="opkg"/> and calls <paramref name="func"/> with <paramref name="user_data"/>.
        /// </summary>
        /// <remarks>New in 1.14.9</remarks>
        public static void ForeachRelation(this GsfInput opkg, GsfOpenPkgIter func, object user_data)
        {
            GsfOpenPkgRels rels = GsfOpenPkgRels.LookupRelations(opkg);
            if (rels == null)
                return;

            pkg_iter_data dat = new pkg_iter_data
            {
                opkg = opkg,
                func = func,
                user_data = user_data,
            };

            foreach (KeyValuePair<string, GsfOpenPkgRel> rel in rels.RelationsById)
            {
                ForeachRelationImpl(rel.Key, rel.Value, dat);
            }
        }

        /// <summary>
        /// Open @opkg's relation <paramref name="id"/>
        /// </summary>
        /// <param name="id">Target id</param>
        /// <returns>A new GsfInput or null, and sets <paramref name="err"/> if possible.</returns>
        /// <remarks>New in 1.14.7</remarks>
        public static GsfInput RelationById(this GsfInput opkg, string id, ref Exception err)
        {
            GsfOpenPkgRel rel = GsfOpenPkgRel.LookupRelationById(opkg, id);
            if (rel != null)
                return GsfOpenPkgRel.OpenRelatedPackage(opkg, rel, ref err);

            err = new Exception($"Unable to find part id='{id}' for '{opkg.Name}'");
            return null;
        }

        /// <summary>
        /// Open one of <paramref name="opkg"/>'s relationships with type=<paramref name="type"/>.
        /// </summary>
        /// <param name="type">Target type</param>
        /// <returns>A new GsfInput or null, and sets <paramref name="err"/> if possible.</returns>
        /// <remarks>New in 1.14.9</remarks>
        public static GsfInput RelationByType(this GsfInput opkg, string type, ref Exception err)
        {
            GsfOpenPkgRel rel = GsfOpenPkgRel.LookupRelationByType(opkg, type);
            if (rel != null)
                return GsfOpenPkgRel.OpenRelatedPackage(opkg, rel, ref err);

            err = new Exception($"Unable to find part with type='{type}' for '{opkg.Name}'");
            return null;
        }

        /// <summary>
        /// Convenience function to parse a related part.
        /// </summary>
        /// <param name="id">Target id</param>
        /// <returns>null on success or an Exception on failure.</returns>
        public static Exception ParseRelationById(this GsfXMLIn xin, string id, XmlDocumentType dtd, XmlNameTable ns)
        {
            if (xin == null)
                return null;

            GsfInput cur_stream = xin.Input;
            if (id == null)
                return new Exception($"Missing id for part in '{cur_stream.Name}'");

            Exception res = null;
            GsfInput part_stream = RelationById(cur_stream, id, ref res);
            if (part_stream != null)
            {
                XmlDocument doc = new XmlDocument(ns);
                doc.AppendChild(dtd);

                // TODO: Enable parsing
                //if (!doc.Parse(part_stream, xin.UserState))
                //    res = new Exception($"Part '{id}' in '{part_stream.Name}' from '{cur_stream.Name}' is corrupt!");
            }

            return res;
        }

        #endregion

        #region Utilties

        private static void ForeachRelationImpl(object id, GsfOpenPkgRel rel, pkg_iter_data dat)
        {
            dat.func(dat.opkg, rel, dat.user_data);
        }

        internal static void WriteContentDefault(this GsfXMLOut xml, string ext, string type)
        {
            xml.StartElement("Default");
            xml.AddString("Extension", ext);
            xml.AddString("ContentType", type);
            xml.EndElement();
        }

        internal static void WriteContentOverride(this GsfXMLOut xml, GsfOutfileOpenPkg open_pkg, string baseName)
        {
            foreach (GsfOutfileOpenPkg child in open_pkg.Children)
            {
                string path;
                if (child.IsDir)
                {
                    path = $"{child.Name}/";
                    xml.WriteContentOverride(child, path);
                }
                else
                {
                    path = $"{baseName}{child.Name}";

                    // Rels files do need content types, the defaults handle them
                    if (child.ContentType != null)
                    {
                        xml.StartElement("Override");
                        xml.AddString("PartName", path);
                        xml.AddString("ContentType", child.ContentType);
                        xml.EndElement();
                    }
                }
            }

            // Dispose of children here to break link cycles.
            open_pkg.Children.Clear();
            open_pkg.Children = null;
        }

        #endregion
    }
}
