using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using SsmsLite.Core.Integration;

namespace SsmsLite.Sync
{
    class SqlServerParser
    {
        private readonly PackageProvider _packageProvider;
        private readonly Location _location;
        private Token[] _tokens;


        public SqlServerParser(PackageProvider packageProvider)
        {
            _packageProvider = packageProvider;
            _location = LocationsHelper.CurrentLocation(_packageProvider.TextDocument);
        }

        public string[] FindCurrentObject()
        {
            var parseResult = Parser.Parse(_packageProvider.AllText);
            if (parseResult == null) return null;

            var sqlBatch = GetCurrentBatch(parseResult.Script);
            if (sqlBatch == null) return null;
            var sqlStatement = GetCurrentStatement(sqlBatch);
            if (sqlStatement == null) return null;

            var tokens = GetCurrentToken(sqlStatement);

            return tokens;
        }

        private SqlBatch GetCurrentBatch(SqlScript sql)
        {
            return sql.Batches
                .FirstOrDefault(b => LocationsHelper.Contain(_location, b.StartLocation, b.EndLocation));
        }

        private SqlStatement GetCurrentStatement(SqlBatch batch)
        {
            return batch.Statements
                .FirstOrDefault(s => LocationsHelper.Contain(_location, s.StartLocation, s.EndLocation));
        }

        private string[] GetCurrentToken(SqlStatement statement)
        {
            _tokens = statement.Tokens.Where(tk => tk.IsSignificant).ToArray(); //&& tk.Type == "TOKEN_ID"

            for (int i = 0; i < _tokens.Length; i++)
            {
                if (LocationsHelper.Contain(_location, _tokens[i].StartLocation, _tokens[i].EndLocation))
                {
                    if (_tokens[i].Type != "TOKEN_ID") return null;
                    var lastIndex = GoLast(i);
                    return ProcessTokensDown(lastIndex);
                }
            }

            return null;
        }

        private int GoLast(int i)
        {
            while (_tokens.Length > i)
            {
                if (_tokens[i].Type != "TOKEN_ID" &&
                    _tokens[i].Type != "."
                   ) return i;
                i++;
            }

            return _tokens.Length;
        }

        private string[] ProcessTokensDown(int i)
        {
            i--;
            var isToken = true;
            var list = new List<string>();
            while (i >= 0)
            {
                if (_tokens[i].Type != "TOKEN_ID" && _tokens[i].Type != ".") break;
                if (_tokens[i].Type == "TOKEN_ID" && !isToken) break;
                if (_tokens[i].Type == "." && isToken) break;
                isToken = !isToken;
                if (_tokens[i].Type == "TOKEN_ID")
                    list.Add(_tokens[i].Text);
                i--;
            }

            return list.ToArray();
        }
    }
}