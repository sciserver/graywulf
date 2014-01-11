using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

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

        private static Regex contextValueRegex = new Regex(@"\[\@([a-zA-Z]+)\]");        // Matches [@word]
        private static Regex entityNameRegex = new Regex(@"\[\$([a-zA-Z\.]+)\]");        // Matches [$word.word.word]


        #region Member Variables

        private Entity entity;
        private string value;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public Entity Entity
        {
            get { return entity; }
        }

        /// <summary>
        /// Gets or sets the value containing expressions.
        /// </summary>
        [XmlText]
        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Gets the value and resolves expressions.
        /// </summary>
        [XmlIgnore]
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
            return ResolveExpression(entity, value);
        }

        public static string ResolveExpression(Entity entity, string value)
        {
            return ResolveExpression(entity, value, 0);
        }

        private static string ResolveExpression(Entity entity, string value, int level)
        {
            if (level > 5)
            {
                throw new ArgumentException("Too deep recursion in expressions.");
            }

            string res = value;

            foreach (Match m in contextValueRegex.Matches(value))
            {
                string key = m.Groups[1].Value;

                if (StringComparer.InvariantCultureIgnoreCase.Compare(key, "username") == 0)
                {
                    res = res.Replace(m.Value, entity.Context.UserName);
                }
            }

            foreach (Match m in entityNameRegex.Matches(value))
            {
                Entity ee = entity;
                string[] parts = m.Groups[1].Value.Split('.');  // splits into parts along dots

                for (int i = 0; i < parts.Length; i++)
                {
                    if (ee == null)
                    {
                        throw new ArgumentNullException(ExceptionMessages.EntityNullException);
                    }

                    System.Reflection.PropertyInfo prop = ee.GetType().GetProperty(parts[i]);

                    // If the expression is parent, load the parent entity
                    if (string.Compare(parts[i], "Parent", true) == 0)
                    {
                        ee = entity.Parent;
                    }
                    else if (prop == null)
                    {
                        throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value));
                    }
                    else if (i != parts.Length - 1)
                    {
                        try
                        {
                            ee = (Entity)prop.GetValue(ee, null);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value), ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            string v = null;
                            object vv = ee.GetType().GetProperty(parts[i]).GetValue(ee, null);

                            if (vv is ExpressionProperty)
                            {
                                v = ResolveExpression(((ExpressionProperty)vv).Entity, ((ExpressionProperty)vv).Value, level + 1);
                            }
                            else if (vv is string)
                            {
                                v = (string)vv;
                            }
                            else
                            {
                                v = vv.ToString();
                            }

                            res = res.Replace(m.Value, v);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value), ex);
                        }
                    }
                }
            }

            return res;
        }
    }
}
