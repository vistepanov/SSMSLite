using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using SsmsLite.Core.Integration;

namespace SsmsLite.Sync
{
    class SqlScriptParse
    {
        private readonly PackageProvider _packageProvider;
        private readonly Location _location;
        private Token[] _tokens;


        public SqlScriptParse(PackageProvider packageProvider)
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

            var tokens = GetCurrentToken(sqlStatement, parseResult); // доковырять поиск use. 

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

        private string[] GetCurrentToken(SqlStatement statement, ParseResult parseResult)
        {
            _tokens = statement.Tokens.Where(tk => tk.IsSignificant).ToArray(); //&& tk.Type == "TOKEN_ID"

            for (int i = 0; i < _tokens.Length; i++)
            {
                if (LocationsHelper.Contain(_location, _tokens[i].StartLocation, _tokens[i].EndLocation))
                {
                    if (_tokens[i].Type != "TOKEN_ID") return null;
                    var db = LookUse(parseResult, _tokens[i].StartLocation.Offset);
                    var lastIndex = GoLast(i);
                    return ProcessTokensDown(lastIndex, db);
                }
            }

            return null;
        }

        private int GoLast(int i)
        {
            var isToken = _tokens[i].Type == "TOKEN_ID";
            while (_tokens.Length > i)
            {
                if (_tokens[i].Type != "TOKEN_ID" && _tokens[i].Type != ".") return i;
                if (_tokens[i].Type == "TOKEN_ID" && !isToken) return i;
                if (_tokens[i].Type == "." && isToken) return i;
                isToken = !isToken;
                i++;
            }

            return _tokens.Length;
        }

        private string[] ProcessTokensDown(int i, string db)
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

            if (list.Count == 1) 
                list.Add("dbo");
            if (list.Count == 2 && !string.IsNullOrEmpty(db))
                list.Add(db);
            
            return list.ToArray();
        }

        private string LookUse(ParseResult parseResult, int offset)
        {
            var tokens = parseResult.Script.Tokens.Where(t => t.StartLocation.Offset < offset && t.Type != "LEX_WHITE" ).ToArray();
            var i = tokens.Length-1;
            while (i>=0 && tokens[i].Type != "TOKEN_USEDB")
            {
                i--;
            }
            if (i<0) return null;
            i++;
            return tokens[i].Text;

            return null;
        }
    }
}