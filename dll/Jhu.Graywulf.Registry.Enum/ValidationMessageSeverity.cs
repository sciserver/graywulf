using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Severity of the validation message
    /// </summary>
    public enum ValidationMessageSeverity
    {
        /// <summary>
        /// The validation message is a status message.
        /// </summary>
        Status,

        /// <summary>
        /// The validation message is a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// The validation message is an error.
        /// </summary>
        Error
    }
}
