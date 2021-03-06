﻿/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Interface for the <b>Jhu.Graywulf.Registry.ReferencedEntity&lt;&gt;</b> class to enable type independent
    /// view of entity references.
    /// </summary>
    /// <remarks>
    /// The <b>Jhu.Graywulf.Registry.ReferencedEntity&lt;&gt;</b> class is a generic class thus instances are
    /// strongly typed so in order to handle them transparently in the generic <see cref="Entity"/>
    /// class they all implement this interface.
    /// </remarks>
    public interface IEntityReference
    {
        Entity ReferencingEntity { get; set; }

        int ReferenceType { get; }

        string Name { get; set; }

        Guid Guid { get; set; }

        object Value { get; set; }

        bool IsEmpty { get; }

        IEntityReference Clone();

        void Resolve();
    }
}
