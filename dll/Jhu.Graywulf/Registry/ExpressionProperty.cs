using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Class that implements functions to handle properties of entities that may contain
    /// resolvable expressions.
    /// </summary>
    /// <remarks>
    /// The <see cref="Value"/> property contains the actual value of the <b>ExpressionProperty</b>
    /// while <see cref="ResolvedValue"/> evaluates the expressions. A valid context with a an
    /// open connection is required in order to resolve expressions successfully.
    /// </remarks>
    public class ExpressionProperty
    {
        #region Member Variables

        private Entity entity;
        private string value;

        #endregion
        #region Member Access Properties

        public Entity Entity
        {
            get { return entity; }
        }

        /// <summary>
        /// Gets or sets the value containing expressions.
        /// </summary>
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets the value and resolves expressions.
        /// </summary>
        public string ResolvedValue
        {
            get { return ResolveExpressions(); }
        }

        #endregion
        #region Constructors and initializers

        public ExpressionProperty()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructs a new <b>ExpressionProperty</b> class and sets its parent entity.
        /// </summary>
        /// <param name="entity">Parent entity of the expression property.</param>
        public ExpressionProperty(Entity entity)
        {
            InitializeMembers();
            this.entity = entity;
        }

        /// <summary>
        /// Constructs a new <b>ExpressionProperty</b> class and sets its parent entity.
        /// </summary>
        /// <param name="entity">Parent entity of the expression property.</param>
        public ExpressionProperty(Entity entity, string value)
        {
            InitializeMembers();
            this.entity = entity;
            this.value = value;
        }

        /// <summary>
        /// Copy constructor that creates a new <b>ExpressionProperty</b> and copies values
        /// from the passed object.
        /// </summary>
        /// <param name="old">The old object to copy values from.</param>
        public ExpressionProperty(ExpressionProperty old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes private members to their initial values.
        /// </summary>
        private void InitializeMembers()
        {
            this.entity = null;
            this.value = string.Empty;
        }

        /// <summary>
        /// Copies private members from another object.
        /// </summary>
        /// <param name="old">The object to copy from.</param>
        private void CopyMembers(ExpressionProperty old)
        {
            this.entity = old.entity;
            this.value = old.value;
        }

        #endregion

        /// <summary>
        /// Resolves expressions in the Value property.
        /// </summary>
        /// <returns>The string with expression replaced to their resolved values.</returns>
        /// <remarks>
        /// This function requires a valid context with an open connection. Valid expressions
        /// are in the form of [$word.word.word]. Expression can contain Parent which refers
        /// to the parent of the entity.
        /// </remarks>
        private string ResolveExpressions()
        {
            return Util.ResolveExpression(entity, value);
        }
    }
}
