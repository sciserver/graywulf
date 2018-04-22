using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Sql.Schema
{
    public static class Constants
    {
        public static readonly Map<Type, DatabaseObjectType> DatabaseObjectTypes = new Map<Type, DatabaseObjectType>()
        {
            { typeof(DataType), DatabaseObjectType.DataType },
            { typeof(Table), DatabaseObjectType.Table },
            { typeof(View), DatabaseObjectType.View },
            { typeof(TableValuedFunction), DatabaseObjectType.TableValuedFunction },
            { typeof(ScalarFunction), DatabaseObjectType.ScalarFunction },
            { typeof(AggregateFunction), DatabaseObjectType.AggregateFunction },
            { typeof(StoredProcedure), DatabaseObjectType.StoredProcedure },
        };

        public static readonly Dictionary<DatabaseObjectType, DatabaseObjectType> SimpleDatabaseObjectTypes = new Dictionary<DatabaseObjectType, DatabaseObjectType>()
        {
            { DatabaseObjectType.DataType, DatabaseObjectType.DataType },
            { DatabaseObjectType.Table, DatabaseObjectType.Table },
            { DatabaseObjectType.View, DatabaseObjectType.View },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.SqlInlineTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectType.TableValuedFunction },
            { DatabaseObjectType.ScalarFunction,DatabaseObjectType.ScalarFunction},
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectType.ScalarFunction },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectType.ScalarFunction },
            { DatabaseObjectType.ClrAggregateFunction, DatabaseObjectType.AggregateFunction },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectType.StoredProcedure },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectType.StoredProcedure },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectType.StoredProcedure },
        };
        
        /// <summary>
        /// Database object singular names
        /// </summary>
        public static readonly Dictionary<DatabaseObjectType, string> DatabaseObjectsName_Singular = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.DataType, DatabaseObjectNames.DataType_Singular },
            { DatabaseObjectType.Table, DatabaseObjectNames.Table_Singular },
            { DatabaseObjectType.View, DatabaseObjectNames.View_Singular },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Singular },
            { DatabaseObjectType.ScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectNames.ScalarFunction_Singular },
            { DatabaseObjectType.ClrAggregateFunction, DatabaseObjectNames.AggregateFunction_Singular },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectNames.StoredProcedure_Singular },
        };

        /// <summary>
        /// Database object plural names
        /// </summary>
        public static readonly Dictionary<DatabaseObjectType, string> DatabaseObjectsName_Plural = new Dictionary<DatabaseObjectType, string>()
        {
            { DatabaseObjectType.DataType, DatabaseObjectNames.DataType_Plural },
            { DatabaseObjectType.Table, DatabaseObjectNames.Table_Plural },
            { DatabaseObjectType.View, DatabaseObjectNames.View_Plural },
            { DatabaseObjectType.TableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.SqlTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.ClrTableValuedFunction, DatabaseObjectNames.TableValuedFunction_Plural },
            { DatabaseObjectType.ScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.SqlScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.ClrScalarFunction, DatabaseObjectNames.ScalarFunction_Plural },
            { DatabaseObjectType.ClrAggregateFunction, DatabaseObjectNames.AggregateFunction_Plural },
            { DatabaseObjectType.StoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
            { DatabaseObjectType.SqlStoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
            { DatabaseObjectType.ClrStoredProcedure, DatabaseObjectNames.StoredProcedure_Plural },
        };

        // TODO: this all has to go into a unit provide imlementation
        public static readonly List<string> UnitNames = new List<string> {
            "%",
            "A",
            "a",
            "adu",
            "AA",
            "arcmin",
            "arcsec",
            "AU",
            "au",
            "Ba",
            "barn",
            "beam",
            "bin",
            "bit",
            "byte",
            "B",
            "C",
            "cd",
            "chan",
            "count",
            "Crab",
            "ct",
            "cy",
            "d",
            "dB",
            "D",
            "deg",
            "erg",
            "eV",
            "F",
            "g",
            "G",
            "H",
            "h",
            "Hz",
            "J",
            "Jy",
            "K",
            "L_sol",
            "lm",
            "lx",
            "lyr",
            "m",
            "M_sol",
            "mag",
            "mas",
            "min",
            "mol",
            "N",
            "Ohm",
            "ohm",
            "Pa",
            "pc",
            "ph",
            "photon",
            "pix",
            "pixel",
            "R",
            "R_sol",
            "rad",
            "Ry",
            "s",
            "S",
            "sr",
            "T",
            "ta",
            "u",
            "V",
            "voxel",
            "W",
            "Wb",
            "yr"
        };

        public static readonly Dictionary<string, double> UnitPrefixes = new Dictionary<string, double> {
            {"d", 1e-1 },
            {"c", 1e-2},
            {"m", 1e-3},
            {"u", 1e-6},
            {"n", 1e-9},
            {"p", 1e-12},
            {"f", 1e-15},
            {"a", 1e-18},
            {"z", 1e-21},
            {"y", 1e-24},
            {"da", 1e+1 },
            {"h", 1e+2},
            {"k", 1e+3},
            {"M", 1e+6},
            {"G", 1e+9},
            {"T", 1e+12},
            {"P", 1e+15},
            {"E", 1e+18},
            {"Z", 1e+21},
            {"Y", 1e+24}
        };

        public static readonly Dictionary<string, string> SpecialUnitCharsHtml = new Dictionary<string, string>
        {
            { "M_sol","M<sub>&odot;</sub>"},
            { "L_sol","L<sub>&odot;</sub>"},
            { "R_sol","R<sub>&odot;</sub>"},
            { "AA","&#8491;"},
            { "u","&mu;"}
        };

        public static readonly Dictionary<string, string> SpecialUnitCharsLatex = new Dictionary<string, string>
        {
            { "M_sol","M_{\\odot}"},
            { "L_sol","L_{\\odot}"},
            { "R_sol","R_{\\odot}"},
            { "AA","\\AA"},
            { "u","\\mu"}
        };
    }
}
