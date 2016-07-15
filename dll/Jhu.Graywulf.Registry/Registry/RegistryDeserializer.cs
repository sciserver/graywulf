using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    public class RegistryDeserializer : ContextObject
    {
        private bool ignoreDuplicates;

        public bool IgnoreDuplicates
        {
            get { return ignoreDuplicates; }
            set { ignoreDuplicates = value; }
        }

        public RegistryDeserializer(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.ignoreDuplicates = true;
        }

        /// <summary>
        /// Deserializes entities from an XML stream.
        /// </summary>
        /// <param name="input">The TextReader object used for reading from the XML stream.</param>
        /// <returns>An IEnumerable interface to the deserialized objects.</returns>
        public IEnumerable<Entity> Deserialize(TextReader input)
        {
            Registry registry;
            var ser = new XmlSerializer(typeof(Registry));

            // Deserialize object into memory, they don't have GUIDs now
            registry = (Registry)ser.Deserialize(input);

            int depth = 0;
            int count = 0;

            while (count < registry.Entities.Length)
            {
                foreach (var entity in registry.Entities)
                {
                    if (entity.ParentReference.IsEmpty && depth == 0 ||
                        !entity.ParentReference.IsEmpty && depth == entity.ParentReference.Name.Count(c => c == Constants.EntityNameSeparator) + 1)
                    {
                        Console.Error.Write("Deserializing {0}... ", entity.Name);

                        entity.IsDeserializing = true;
                        entity.Context = this.Context;

                        if (!entity.ParentReference.IsEmpty)
                        {
                            ResolveParentReference(entity);
                        }

                        try
                        {
                            entity.Save();
                            Console.Error.WriteLine("done.");
                        }
                        catch (DuplicateNameException)
                        {
                            if (!ignoreDuplicates)
                            {
                                throw;
                            }
                            Console.Error.WriteLine("ignored duplicate.");
                        }
                        catch (Exception)
                        {
                            Console.Error.WriteLine("failed.");
                            throw;
                        }

                        count++;
                    }
                }

                depth++;
            }

            foreach (var entity in registry.Entities)
            {
                Console.Error.Write("Resolving references of {0}... ", entity.Name);

                try
                {
                    entity.IsDeserializing = false;     // Allows saving entity references
                    ResolveNameReferences(entity);
                    entity.Save();

                    Console.Error.WriteLine("done.");
                }
                catch (DuplicateNameException)
                {
                    if (!ignoreDuplicates)
                    {
                        throw;
                    }
                    Console.Error.WriteLine("ignored duplicate.");
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("failed.");
                    throw;
                }
            }

            return registry.Entities;
        }

        private void ResolveParentReference(Entity entity)
        {
            entity.ParentReference.EnsureEntityLoaded();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This is used by the XML deserializer
        /// </remarks>
        private void ResolveNameReferences(Entity entity)
        {
            foreach (IEntityReference r in entity.EntityReferences.Values)
            {
                if (!r.IsEmpty)
                {
                    // Make sure entity reference is loaded by retrieving its value
                    var o = r.Value;
                }
            }

            entity.IsEntityReferencesLoaded = true;
        }
    }
}
