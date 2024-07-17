export interface IRenderTree {
    parentId: string | null;
    hubId: string | null;
    projectId: string | null;
    id: string;
    name: string;
    type: string | null;
    data: any;
    children: IRenderTree[];
    isLoaded: boolean;
    isLoading: boolean;
}

export interface NodeVisitorFunction {
    (visitingNode: IRenderTree): boolean;
}

export class RenderTree {
    constructor(private root: IRenderTree) {}

    find(id: string): IRenderTree | undefined {
        let first;

        const callback = (node: IRenderTree) => {
            if (node.id === id) {
                first = node;
                return false;
            }

            return true;
        };

        this.search(this.root, callback);

        return first;
    }

    search(node: IRenderTree, callback: NodeVisitorFunction): boolean {
        const len = node.children?.length;
        let keepGoing = callback(node);

        for (let i = 0; i < len; i++) {
            if (keepGoing === false) {
                return false;
            }

            keepGoing = this.search(node.children[i], callback);
        }

        return keepGoing;
    }

    searchPath(
        node: IRenderTree,
        stack: IRenderTree[],
        callback: NodeVisitorFunction
    ): boolean {
        stack.push(node);

        const len = node.children?.length;
        let found = callback(node);

        for (let i = 0; i < len; i++) {
            if (found) {
                return true;
            }

            found = this.searchPath(node.children[i], stack, callback);
        }

        if (!found) {
            stack.pop();
        }

        return found;
    }

    path(target: IRenderTree): IRenderTree[] {
        let stack: IRenderTree[] = [];

        const callback = (node: IRenderTree) => {
            return (node.id === target.id);
        };

        this.searchPath(this.root, stack, callback);

        return stack;
    }
}
