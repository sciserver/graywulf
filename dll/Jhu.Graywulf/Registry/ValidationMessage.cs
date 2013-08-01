/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents a message that is sent my the <see cref="Entity.Validate" /> function of the <see cref="Entity" /> class
    /// </summary>
    /// <remarks>
    /// Always use the <see cref="ValidationMessages" /> resource class for text messages instead of hardcoding them.
    /// </remarks>
    public class ValidationMessage
    {
        #region Member Variables

        private Entity entity;
        private ValidationMessageSeverity severity;
        private string message;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets the entity that sent the validation message.
        /// </summary>
        public Entity Entity
        {
            get { return entity; }
        }

        /// <summary>
        /// Gets or sets the property that refers to the severity of the validation message
        /// </summary>
        public ValidationMessageSeverity Severity
        {
            get { return severity; }
            set { severity = value; }
        }

        /// <summary>
        /// Gets or sets the free-text message associated with the validation message.
        /// </summary>
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Creates a new validation message class.
        /// </summary>
        /// <param name="entity">Reference to the entity that is sending the validation message</param>
        /// <param name="severity">Severity of the message</param>
        /// <param name="message">Free-text description of the message</param>
        public ValidationMessage(Entity entity, ValidationMessageSeverity severity, string message)
        {
            this.entity = entity;
            this.severity = severity;
            this.message = message;
        }

        #endregion
    }
}
