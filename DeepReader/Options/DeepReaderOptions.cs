using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Other;

namespace DeepReader.Options
{
    public class DeepReaderOptions
    {
        public int DlogBlockSegmentListSize { get; set; }
        public int DlogReaderBlockQueueSize { get; set; }
        public int FlatteningMaxDegreeOfParallelism { get; set; }
        public int DlogParserTasks { get; set; }

        public int BlockProcessingTasks { get; set; }
        public bool FilterEmptyTransactions { get; set; } = true;

        public Filter Filter { get; set; }
    }

    public class Filter
    {
        public List<string> Actions { get; set; } = new();
        public List<string> Deltas { get; set; } = new();

        public Func<ActionTrace, bool> BuildActionFilter()
        {
            List<ActionFilter> actionFilter = new();
            foreach (var contractAction in Actions)
            {
                var splitted = contractAction.Split("::");
                var contract = splitted[0];
                var action = splitted.Length > 1 ? splitted[1] : "*";

                actionFilter.Add(new ActionFilter(contract, action));
            }
            if(actionFilter.Count > 0)
                return trace => !actionFilter.Any(filter =>
                    filter.Contract.Equals(trace.Act.Account) &&
                    (filter.Action == trace.Act.Name || filter.Action == Name.TypeWildcard));
            return _ => true;
        }

        public Func<ExtendedDbOp, bool> BuildDeltaFilter()
        {
            List<DeltaFilter> deltaFilter = new();
            foreach (var contractAction in Deltas)
            {
                var splitted = contractAction.Split("::");
                var code = splitted[0];
                var scope = splitted.Length > 1 ? splitted[1] : "*";
                var table = splitted.Length > 2 ? splitted[2] : "*";

                deltaFilter.Add(new DeltaFilter(code, scope, table));
            }

            if(deltaFilter.Count > 0)
                return dbOp => !deltaFilter.Any(filter =>
                    filter.Code.Equals(dbOp.Code) &&
                    (filter.Scope == dbOp.Scope || filter.Scope == Name.TypeWildcard) &&
                    (filter.Table == dbOp.TableName || filter.Table == Name.TypeWildcard));
            return _ => true;
        }

        public class ActionFilter
        {
            public Name Contract { get; }
            public Name Action { get; }
            public ActionFilter(Name contract, Name action)
            {
                Contract = contract;
                Action = action;
            }

            public ActionFilter(string contract, string action)
            {
                Contract = contract != "*" ? NameCache.GetOrCreate(contract, false) : Name.TypeWildcard;
                Action = action != "*" ? NameCache.GetOrCreate(action, false) : Name.TypeWildcard;
            }
        }

        public class DeltaFilter
        {
            public Name Code { get; }

            public Name Table { get; }

            public Name Scope { get; }
            public DeltaFilter(Name code, Name table, Name scope)
            {
                Code = code;
                Table = table;
                Scope = scope;
            }

            public DeltaFilter(string code, string table, string scope)
            {
                Code = code != "*" ? NameCache.GetOrCreate(code, false) : Name.TypeWildcard;
                Table = table != "*" ? NameCache.GetOrCreate(table, false) : Name.TypeWildcard;
                Scope = scope != "*" ? NameCache.GetOrCreate(scope, false) : Name.TypeWildcard;
            }
        }
    }
}
