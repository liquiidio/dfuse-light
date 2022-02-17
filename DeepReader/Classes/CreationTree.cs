using DeepReader.Types;
using DeepReader.Types.Eosio.Chain;

namespace DeepReader.Classes;

public static class CreationTree
{
    public static List<Node> ComputeCreationTree(List<CreationOp> creationOps)
    {
        // TODO, not sure if this is converted correctly from dfuse-code

        if (creationOps.Count <= 0)
            return new List<Node>();

        var actionIndex = -1;
        var opsMap = CreationOpsToMap(creationOps);

        var roots = new List<Node>();
        var opKinds = opsMap[actionIndex + 1];

        foreach (var ok in opKinds)
        {
            if (opKinds[0] != "ROOT") {
                // TODO return nil, fmt.Errorf("first exec op kind of execution start should be ROOT, got %s", opKinds[0])
            }

            var root = new Node { Kind = "ROOT", ActionIndex = -1, Children = new List<Node>() };
            roots.Add(root);

            ExecuteAction(actionIndex, root, opsMap);

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

    private static void ExecuteAction(int actionIndex, Node root, Dictionary<int, string[]> opsMap)
    {
        actionIndex++;
        root.ActionIndex = actionIndex;

        var notifies = new List<Node>();
        var cfas = new List<Node>();
        var inlines = new List<Node>();
        RecordChildCreationOp(root, opsMap[root.ActionIndex], ref notifies, ref cfas, ref inlines);

        foreach (var notify in notifies)
        {
            var nestedNotifies = new List<Node>();
            var nestedCfas = new List<Node>();
            var nestedInlines = new List<Node>();

            // TODO pass notifies, cfas, inlines directly?
            ExecuteNotify(ref actionIndex, notify, opsMap, ref nestedNotifies, ref nestedCfas, ref nestedInlines);
            
            notifies.AddRange(nestedNotifies);
            cfas.AddRange(nestedCfas);
            notifies.AddRange(nestedInlines);
        }

        foreach (var cfa in cfas)
        {
            ExecuteAction(actionIndex, cfa, opsMap);
        }

        foreach (var inline in inlines)
        {
            ExecuteAction(actionIndex, inline, opsMap);
        }
    }

    private static void ExecuteNotify(ref int actionIndex, Node root, Dictionary<int, string[]> opsMap, ref List<Node> notifies, ref List<Node> cfas, ref List<Node> inlines)
    {
        actionIndex++;
        root.ActionIndex = actionIndex;

        RecordChildCreationOp(root, opsMap[root.ActionIndex], ref notifies, ref cfas, ref inlines);
    }

    private static void RecordChildCreationOp(Node root, string[] opKinds, ref List<Node> notifies, ref List<Node> cfas, ref List<Node> inlines)
    {
        foreach (var opKind in opKinds)
        {
            if (opKind == "ROOT")
            {
                continue;
            }

            var child = new Node() { Kind = opKind, ActionIndex = -1, Children = new List<Node>() };
            switch (opKind)
            {
                case "NOTIFY":
                    notifies.Add(child);
                    break;
                case "CFA_INLINE":
                    cfas.Add(child);
                    break;
                case "INLINE":
                    inlines.Add(child);
                    break;
            }

            root.Children.Add(child);
        }
        
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
        public string Kind;
        public int ActionIndex;
        public IList<Node> Children;
    }
    
    private static Dictionary<int, string[]> CreationOpsToMap(List<CreationOp> creationOps)
    {
        Dictionary<int, string[]> mapping = new Dictionary<int, string[]>();

        for (int i = 0; i < creationOps.Count; i++)
        {
            mapping[i] = new[] { creationOps[i].Kind };
        }
        return mapping;
    }
    
    public static IList<CreationFlatNode> ToFlatTree(List<Node> roots)
    {
        var tree = new List<CreationFlatNode>();

        var walkIndex = -1;
        foreach (var root in roots)
        {
            tree.AddRange(_ToFlatTree(root, -1, ref walkIndex));
        }
        
        return tree.ToArray();
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