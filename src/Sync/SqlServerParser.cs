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

        public string FindCurrentObject()
        {
            var r = Parser.Parse(_packageProvider.AllText);
            if (r == null) return "";

            var b = GetCurrentBatch(r.Script);
            if (b == null) return "";
            var s = GetCurrentStatement(b);
            if (s == null) return "";

            var tokens = GetCurrentToken(s);

            //  MessageBox.Show(s.Sql);
            return string.Join(".", tokens.Reverse());
        }

        private SqlBatch GetCurrentBatch(SqlScript sql)
        {
            return sql.Batches
                .FirstOrDefault(b => LocationsHelper.Contain(_location, b.StartLocation, b.EndLocation));
        }

        private SqlStatement GetCurrentStatement(SqlBatch batch)
        {
            return batch.Statements.FirstOrDefault(s =>
                LocationsHelper.Contain(_location, s.StartLocation, s.EndLocation)
            );
        }

        private IReadOnlyList<Token> GetCurrentToken(SqlStatement statement)
        {
            _tokens = statement.Tokens.Where(tk => tk.IsSignificant).ToArray(); //&& tk.Type == "TOKEN_ID"

            for (int i = 0; i < _tokens.Length; i++)
            {
                if (LocationsHelper.Contain(_location, _tokens[i].StartLocation, _tokens[i].EndLocation))
                {
                    if (_tokens[i].Type != "TOKEN_ID") return new List<Token>();
                    var lastIndex = GoLast(i);
                    return ProcessTokensDown(lastIndex);
                }
            }

            return new List<Token>();
        }

        private int GoLast(int i)
        {
            while (true)
            {
                if (_tokens.Length >= i) return _tokens.Length - 1;
                if (_tokens[i].Type != "TOKEN_ID") return i;
                i++;
            }
        }

        private IReadOnlyList<Token> ProcessTokensDown(int i)
        {
            var list = new List<Token>();
            while (i >= 0)
            {
                if (_tokens[i].Type != "TOKEN_ID" && _tokens[i].Type != ".") return list;
                if (_tokens[i].Type == "TOKEN_ID")
                    list.Add(_tokens[i]);
                i--;
            }

            return list;
        }
    }
}