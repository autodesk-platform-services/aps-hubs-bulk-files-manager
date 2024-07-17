import Button from "@mui/material/Button";
import React, {useState} from "react";
import {Dialog} from "@mui/material";
import {ICloudFolderProps, ProjectTree} from "./projectTree.tsx";

export function BrowseCloudLocation({onFolderChange, type}: {
    type: 'upload' | 'download';
    onFolderChange: ({hubId, projectId, folderId, folderPath}: ICloudFolderProps) => void}) {
    const [openDiag, setOpenDiag] = useState<boolean>(false);

    return (
        <>
            <Button size={'large'}
                variant={"outlined"}
                onClick={() => setOpenDiag(true)}
            >Browse cloud location
            </Button>

            <Dialog
                open={openDiag}
                fullWidth={true}
                onClose={()=>setOpenDiag(false)}
            >
                <ProjectTree
                    type={type}
                    onChange={onFolderChange}
                    onSubmit={() => setOpenDiag(false)}
                />
            </Dialog>
        </>
    )
}