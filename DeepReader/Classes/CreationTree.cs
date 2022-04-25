using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using KGySoft.CoreLibraries;
using Serilog;

namespace DeepReader.Classes;

public static class CreationTree
{
    public static List<Node> ComputeCreationTree(IReadOnlyList<CreationOp> creationOps)
    {
        // TODO, not sure if this is converted correctly from dfuse-code

        if (creationOps.Count <= 0)
            return new List<Node>();

        var actionIndex = -1;
        var opsMap = CreationOpsToMap(creationOps);

        var roots = new List<Node>();

        var ok = opsMap.Length > (actionIndex + 1);
        var opKinds = opsMap[actionIndex + 1];

        while (ok)
        {
            if (opKinds.First() != CreationOpKind.ROOT)
            {
                Log.Warning($"first exec op kind of execution start should be ROOT, got {opKinds.First()}");
            }

            var root = new Node { Kind = CreationOpKind.ROOT, ActionIndex = -1, Children = new List<Node>() };
            roots.Add(root);

            ExecuteAction(ref actionIndex, root, opsMap);

//            opKinds = opsMap[actionIndex + 1];

            if (opsMap.Length <= (actionIndex + 1))
                ok = false;
            else
                opKinds = opsMap[actionIndex + 1];
            // TODO: We should check for gaps in action indices here. Assume an exec ops
            //       list of `[{ROOT, 0}, {NOTIFY, 1}, {ROOT, 2}]`. In this list, we would
            //       create a ROOT #0, skip NOTIFY then try to execute ROOT #2. This is incorrect
            //       and there is a gap, i.e. there is an action index lower than next 2 that is
            //       not part of previous tree. How exactly we would do it is unsure, but that would
            //       add a validation step that everything is kosher.
        }

        return roots;
    }

    private static void ExecuteAction(ref int actionIndex, Node root, CreationOpKind[][] opsMap)
    {
        actionIndex++;
        root.ActionIndex = actionIndex;

        var (notifies, cfas, inlines) = RecordChildCreationOp(root, opsMap[root.ActionIndex]);

        for(var i = 0; i < notifies.Count; i++)
        {
            if (opsMap.Length > actionIndex + 1)
            {
                var (nestedNotifies, nestedCfas, nestedInlines) = ExecuteNotify(ref actionIndex, notifies[i], opsMap);

                notifies.AddRange(nestedNotifies);
                cfas.AddRange(nestedCfas);
                inlines.AddRange(nestedInlines);
            }
            else
                break; // TODO, here seems to be something wrong
        }

        foreach (var cfa in cfas)
        {
            if (opsMap.Length > actionIndex + 1)
            {
                ExecuteAction(ref actionIndex, cfa, opsMap);
            }
            else
                break; // TODO, here seems to be something wrong
        }

        foreach (var inline in inlines)
        {
            if (opsMap.Length > actionIndex + 1)
            {
                ExecuteAction(ref actionIndex, inline, opsMap);
            }
            else
                break; // TODO, here seems to be something wrong
        }
    }

    private static (ICollection<Node> notifies, ICollection<Node> cfas, ICollection<Node> inlines) ExecuteNotify(ref int actionIndex, Node root, CreationOpKind[][] opsMap)
    {
        actionIndex++;
        root.ActionIndex = actionIndex;

        return RecordChildCreationOp(root, opsMap[root.ActionIndex]);
    }

    private static (IList<Node> notifies, IList<Node> cfas, IList<Node> inlines) RecordChildCreationOp(Node root, CreationOpKind[] opKinds)
    {
        var notifies = new List<Node>(); var cfas = new List<Node>(); var inlines = new List<Node>();
        foreach (var opKind in opKinds)
        {
            var child = new Node() { Kind = opKind, ActionIndex = -1, Children = new List<Node>() };
            switch (opKind)
            {
                case CreationOpKind.ROOT:
                    continue;
                case CreationOpKind.NOTIFY:
                    notifies.Add(child);
                    break;
                case CreationOpKind.CFA_INLINE:
                    cfas.Add(child);
                    break;
                case CreationOpKind.INLINE:
                    inlines.Add(child);
                    break;
            }

            root.Children.Add(child);
        }
        
        return (notifies, cfas, inlines);

        // for _, opKind := range opKinds {
        //     if opKind == "ROOT" {
        //         continue
        //     }
        //
        //     child := &node{opKind, -1, nil}
        //     switch opKind {
        //         case "NOTIFY":
        //         notifies = append(notifies, child)
        //         case "CFA_INLINE":
        //         cfas = append(cfas, child)
        //         case "INLINE":
        //         inlines = append(inlines, child)
        //     }
        //
        //     root.children = append(root.children, child)
        // }
    }

    public struct Node
    {
        public CreationOpKind Kind;
        public int ActionIndex;
        public IList<Node> Children;
    }
    
    private static CreationOpKind[][] CreationOpsToMap(IReadOnlyList<CreationOp> creationOps)
    {
        var i = 0;  // TODO here's something wrong
        return creationOps.GroupBy(o => o.ActionIndex).Select(o => o.Select(o => o.Kind).ToArray()).ToArray();
        //Dictionary<int, CreationOpKind> mapping = new Dictionary<int, CreationOpKind>();

        //for (int i = 0; i < creationOps.Count; i++)
        //{
        //    mapping[i] = new[] { creationOps[i].Kind };
        //}

        //return mapping;
    }
    
    public static IList<CreationFlatNode> ToFlatTree(List<Node> roots)
    {
        var tree = new List<CreationFlatNode>();

        var walkIndex = -1;
        foreach (var root in roots)
        {
            tree.AddRange(_ToFlatTree(root, -1, ref walkIndex));
        }
        
        return tree;
    }

    private static IList<CreationFlatNode> _ToFlatTree(Node root, int parentIndex, ref int walkIndex)
    {
        var tree = new List<CreationFlatNode>(){ new(){ WalkIndex = walkIndex, CreatorActionIndex = parentIndex, ExecutionActionIndex = root.ActionIndex}};
        var childRootIndex = walkIndex;

        foreach (var child in root.Children)
        {
            walkIndex++;
            tree.AddRange(_ToFlatTree(child, childRootIndex, ref walkIndex));
        }

        return tree;
    }
}