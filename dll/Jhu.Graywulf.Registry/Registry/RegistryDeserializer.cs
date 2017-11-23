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
        private DuplicateMergeMethod duplicateMergeMethod;

        public DuplicateMergeMethod DuplicateMergeMethod
        {
            get { return duplicateMergeMethod; }
            set { duplicateMergeMethod = value; }
        }

        public RegistryDeserializer(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.duplicateMergeMethod = DuplicateMergeMethod.Ignore;
        }

        /// <summary>
        /// Deserializes entities from an XML stream.
        /// </summary>
        /// <param name="input">The TextReader object used for reading from the XML stream.</param>
        /// <returns>An IEnumerable interface to the deserialized objects.</returns>
        public IEnumerable<Entity> Deserialize(TextReader input)
        {
            var ser = new XmlSerializer(typeof(Registry));
            var registry = (Registry)ser.Deserialize(input);
            return DeserializeRegistry(registry);
        }

        private IEnumerable<Entity> DeserializeRegistry(Registry registry)
        {
            int depth = 0;
            int count = 0;

            while (count < registry.Entities.Length)
            {
                foreach (var entity in registry.Entities)
                {
                    if (entity.ParentReference.IsEmpty && depth == 0 ||
                        !entity.ParentReference.IsEmpty && depth == entity.ParentReference.Name.Count(c => c == Constants.EntityNameSeparator) + 1)
                    {
                        DeserializeEntity(entity);
                        count++;
                    }
                }

                depth++;
            }

            foreach (var entity in registry.Entities)
            {
                ResolveReferences(entity);
            }

            return registry.Entities;
        }

        private void DeserializeEntity(Entity entity)
        {
            // Console.Error.Write("Deserializing {0}... ", entity.Name);

            entity.IsDeserializing = true;
            entity.RegistryContext = this.RegistryContext;

            if (!entity.ParentReference.IsEmpty)
            {
                ResolveParentReference(entity);
            }

            SaveEntity(entity);
        }

        private void ResolveReferences(Entity entity)
        {
            Console.Error.WriteLine("Resolving references of {0}... ", entity.Name);

            // Allow saving entity references
            entity.IsDeserializing = false;
            entity.IsEntityReferencesLoaded = true;

            ResolveNameReferences(entity);
            SaveEntity(entity);
        }

        private void SaveEntity(Entity entity)
        {
            try
            {
                entity.Save();
                //Console.Error.WriteLine("done.");
            }
            catch (DuplicateNameException)
            {
                switch (duplicateMergeMethod)
                {
                    case DuplicateMergeMethod.Ignore:
                        Console.Error.WriteLine("ignored duplicate.");
                        break;
                    case DuplicateMergeMethod.Update:
                        UpdateEntity(entity);
                        Console.Error.WriteLine("updated duplicate.");
                        break;
                    case DuplicateMergeMethod.Fail:
                        throw;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("failed.");
                throw;
            }
        }

        private void UpdateEntity(Entity entity)
        {
            var ef = new EntityFactory(RegistryContext);
            var e = ef.LoadEntity(entity.GetFullyQualifiedName());
            e.UpdateMembers(entity);
            e.Save();
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
                    r.Resolve();
                }
            }
        }
    }
}
