import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import { useEffect, useState } from "react";
import { observer } from "mobx-react";
import {
    useTopFolders,
    useFolderContents,
    ISimpleDmResponse
} from "../Utilities/Hooks/useDataManagement";
import ArrowDropDownIcon from "@mui/icons-material/ArrowDropDown";
import ArrowRightIcon from "@mui/icons-material/ArrowRight";
import TreeView from "@mui/lab/TreeView";
import TreeItem, { TreeItemProps, treeItemClasses } from "@mui/lab/TreeItem";
import { styled } from "@mui/material/styles";
import { IRenderTree, RenderTree } from "../Utilities/RenderTree";
import InsertDriveFileIcon from "@mui/icons-material/InsertDriveFile";
import Loading from "./Loading";
import * as React from "react";

declare module "react" {
    interface CSSProperties {
        "--tree-view-color"?: string;
        "--tree-view-bg-color"?: string;
    }
}

const docRoot: IRenderTree = {
    parentId: null,
    hubId: "root",
    projectId: "",
    id: "root",
    name: "",
    type: "",
    data: null,
    children: [],
    isLoaded: false,
    isLoading: false
};

export interface FolderSelectorProps {
    hub?: any;
    project?: any;
    onChange: (nodeId: string, folderPath: string) => void;
}

const FolderSelector = observer((props: FolderSelectorProps) => {
    const { hub, project } = props;
    const [docs, setDocs] = useState<IRenderTree>({ ...docRoot });
    const [expanded, setExpanded] = useState<string[]>([]);
    const [expandAll, setExpandAll] = useState<string[]>([]);
    const [folderId, setFolderId] = useState<string>("");
    
    const { isFetching: isTopFoldersLoading, data: topFolders } = useTopFolders(
        hub?.id,
        project?.id,
        {
            refetchOnMount: true,
            enabled: hub?.id && project?.id && !folderId,
            onSuccess: (data: ISimpleDmResponse[]) => {

                const docs: IRenderTree = {
                    ...docRoot,
                    hubId: hub.id,
                    projectId: project.id,
                    name: project.name,
                    children: [],
                    isLoaded: false,
                    isLoading: false,
                };

                data?.filter((n) => n.id.includes("fs.folder"))
                    .forEach((item: ISimpleDmResponse) => {
                        docs.children.push({
                            parentId: docs.id,
                            hubId: docs.hubId,
                            projectId: docs.projectId,
                            id: item.id,
                            name: item.name,
                            type: "folder",
                            data: item,
                            children: [],
                            isLoaded: false,
                            isLoading: false,
                        });
                    });

                docs.children.sort((a, b) => a.name.localeCompare(b.name));
                setExpanded([...expanded, docs.id]);
                setDocs({ ...docs });
                setFolderId(docs.children[0]?.id);
            },
        }
    );

    const { isFetching: isFolderContentsLoading, data: folderContents } =
        useFolderContents(project?.id, folderId, {
            refetchOnMount: true,
            enabled: project?.id && !!folderId,
            onSuccess: (data: ISimpleDmResponse[]) => {

                const nodes = new RenderTree(docs);
                const node = nodes.find(folderId);
                if(!node) return;
                
                node.isLoading = false;

                if (node.isLoaded) {
                    return;
                }

                data?.filter((n) => n.id.includes("fs.folder"))
                    .forEach((item: ISimpleDmResponse) => {
                        node.children.push({
                            parentId: docs.id,
                            hubId: docs.hubId,
                            projectId: docs.projectId,
                            id: item.id,
                            name: item.name,
                            type: "folder",
                            data: item,
                            children: [],
                            isLoaded: false,
                            isLoading: false,
                        });
                    });

                node.isLoaded = true;
                node.children.sort((a, b) => a.name.localeCompare(b.name));
                setExpanded([...expanded, node.id]);
                setDocs({ ...docs });
            },
        });

    const handleToggle = (event: React.SyntheticEvent, nodeIds: string[]) => {
        setExpanded(nodeIds);
    };

    const handleSelect = (event: React.SyntheticEvent, nodeId: string) => {
        const nodes = new RenderTree(docs);
        const node = nodes.find(nodeId);
        if(!node) return;

        const stack = nodes.path(node);
        
        
      // stack.shift(); // Remove the project name from the path
        const folderPath = stack.reduce((acc, value) => {
            return acc + `\\${value.name}`;
        }, '').slice(1); // Remove the leading '\' from the path

        if (!node.isLoaded) {
            node.isLoading = true;
        }

        setFolderId(nodeId);
        props.onChange(nodeId, folderPath);
    };

    useEffect(() => {
        if (hub?.id !== docs.hubId || project?.id !== docs.projectId) {
            setFolderId("");
            setDocs({
                ...docs,
                hubId: hub.id,
                projectId: project.id,
                name: project.name,
                children: [],
            });
        }
    }, [hub?.id, project?.id]);

    return (
        <>
            {isTopFoldersLoading ? (
                <Box
                    sx={{
                        display: "flex",
                        flexGrow: 1,
                        alignItems: "center",
                        justifyContent: "center",
                    }}
                >
                    <Loading size={20} />
                </Box>
            ) : (
                <TreeView
                    aria-label="rich object"
                    expanded={expanded}
                    onNodeToggle={handleToggle}
                    onNodeSelect={handleSelect}
                    defaultCollapseIcon={<ArrowDropDownIcon />}
                    defaultExpandIcon={<ArrowRightIcon />}
                    defaultEndIcon={<div style={{ width: 24 }} />}
                    sx={{
                        flexGrow: 1,
                        maxWidth: 600,
                        overflowX: "hidden",
                        overflowY: "auto",
                        color: "rgb(34, 34, 34)",
                    }}
                >
                    {renderTree(docs)}
                </TreeView>
            )}
        </>
    );
});

const StyledTreeItemRoot = styled(TreeItem)(({ theme }) => ({
    color: theme.palette.text.secondary,
    [`& .${treeItemClasses.content}`]: {
        color: theme.palette.text.secondary,
        fontWeight: theme.typography.fontWeightMedium,
        "&:hover": {
            backgroundColor: theme.palette.action.hover,
        },
        "&.Mui-focused, &.Mui-selected, &.Mui-selected.Mui-focused": {
            backgroundColor: `var(--tree-view-bg-color, ${theme.palette.action.selected})`,
            color: "var(--tree-view-color)",
        },
        [`& .${treeItemClasses.label}`]: {
            fontWeight: "inherit",
            color: "inherit",
        },
    },
    [`& .${treeItemClasses.group}`]: {
        marginLeft: '.5em',
        // [`& .${treeItemClasses.content}`]: {
        //     paddingLeft: theme.spacing(2),
        // },
    },
}));

type StyledTreeItemProps = TreeItemProps & {
    bgColor?: string;
    color?: string;
    labelText: string;
    isLoading: boolean;
};

const StyledTreeItem = (props: StyledTreeItemProps) => {
    const { bgColor, color, labelText, isLoading, ...other } = props;

    return (
        <StyledTreeItemRoot
            label={
                <Box sx={{ display: "flex", alignItems: "center", p: 0.5, pr: 0 }}>
                    <InsertDriveFileIcon />
                    <Typography
                        variant="body2"
                        color="inherit"
                        sx={{
                            flexGrow: 1,
                            fontWeight: "inherit",
                            wordBreak: "break-all",
                            marginRight: "20px",
                        }}
                    >
                        {labelText}
                    </Typography>
                    {isLoading && (
                        <Box sx={{ display: "flex", alignItems: "center", width: "42px" }}>
                            <Loading size={20} />
                        </Box>
                    )}
                </Box>
            }
            style={{
                "--tree-view-color": color,
                "--tree-view-bg-color": bgColor,
            }}
            {...other}
        />
    );
};

const renderTree = (node: IRenderTree) => {
    const children = Array.isArray(node.children)
        ? node.children.map((child: IRenderTree) => renderTree(child))
        : null;

    return (
        <Box key={node.id}>
            {node.id === "root" ? (
                <Box key={node.id}>{children}</Box>
            ) : (
                <StyledTreeItem
                    key={node.id}
                    nodeId={node.id}
                    labelText={node.name}
                    isLoading={node.isLoading}
                >
                    {children}
                </StyledTreeItem>
            )}
        </Box>
    );
};

export default FolderSelector;
