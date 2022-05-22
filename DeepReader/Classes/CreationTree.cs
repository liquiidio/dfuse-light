using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.Other;
using KGySoft.CoreLibraries;
using Serilog;

namespace DeepReader.Classes;

public static class CreationTree
{
    public static List<CreationTreeNode> ComputeCreationTree(IReadOnlyList<CreationOp> creationOps)
    {
        // TODO, not sure if this is converted correctly from dfuse-code

        if (creationOps.Count <= 0)
            return new List<CreationTreeNode>();

        var actionIndex = -1;
        var opsMap = CreationOpsToMap(creationOps);

        var roots = new List<CreationTreeNode>();

        var ok = opsMap.Length > (actionIndex + 1);
        var opKinds = opsMap[actionIndex + 1];

        while (ok)
        {
            if (opKinds.First() != CreationOpKind.ROOT)
            {
                Log.Warning($"first exec op kind of execution start should be ROOT, got {opKinds.First()}");
            }

            var root = new CreationTreeNode { Kind = CreationOpKind.ROOT, ActionIndex = -1, Children = new List<CreationTreeNode>() };
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

    private static void ExecuteAction(ref int actionIndex, CreationTreeNode root, CreationOpKind[][] opsMap)
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

    private static (ICollection<CreationTreeNode> notifies, ICollection<CreationTreeNode> cfas, ICollection<CreationTreeNode> inlines) ExecuteNotify(ref int actionIndex, CreationTreeNode root, CreationOpKind[][] opsMap)
    {
        actionIndex++;
        root.ActionIndex = actionIndex;

        return RecordChildCreationOp(root, opsMap[root.ActionIndex]);
    }

    private static (IList<CreationTreeNode> notifies, IList<CreationTreeNode> cfas, IList<CreationTreeNode> inlines) RecordChildCreationOp(CreationTreeNode root, CreationOpKind[] opKinds)
    {
        var notifies = new List<CreationTreeNode>(); var cfas = new List<CreationTreeNode>(); var inlines = new List<CreationTreeNode>();
        foreach (var opKind in opKinds)
        {
            var child = new CreationTreeNode() { Kind = opKind, ActionIndex = -1, Children = new List<CreationTreeNode>() };
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
    
    private static CreationOpKind[][] CreationOpsToMap(IReadOnlyList<CreationOp> creationOps)
    {
        // TODO here's something wrong
        return creationOps.GroupBy(o => o.ActionIndex).Select(o => o.Select(go => go.Kind).ToArray()).ToArray();
        //Dictionary<int, CreationOpKind> mapping = new Dictionary<int, CreationOpKind>();

        //for (int i = 0; i < creationOps.Count; i++)
        //{
        //    mapping[i] = new[] { creationOps[i].Kind };
        //}

        //return mapping;
    }
    
    public static IList<CreationFlatNode> ToFlatTree(List<CreationTreeNode> roots)
    {
        var tree = new List<CreationFlatNode>();

        var walkIndex = -1;
        foreach (var root in roots)
        {
            tree.AddRange(_ToFlatTree(root, -1, ref walkIndex));
        }
        
        return tree;
    }

    private static IList<CreationFlatNode> _ToFlatTree(CreationTreeNode root, int parentIndex, ref int walkIndex)
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