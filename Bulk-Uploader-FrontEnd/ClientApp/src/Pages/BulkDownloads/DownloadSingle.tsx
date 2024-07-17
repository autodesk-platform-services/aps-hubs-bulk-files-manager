import {useNavigate, useParams} from "react-router";
import {useBulkDownload} from "../../Utilities/Hooks/useBulkDownload";
import {BulkDownloadSingleHeader} from "./BulkDownloadSingleHeader";
import React from "react";
import Button from "@mui/material/Button";
import {useBulkDownloads} from "../../Utilities/Hooks/useBulkDownloads";
import Box from "@mui/material/Box";
import {bytesToSize, DownloadSingleStatus} from "./DownloadSingleStatus";
import {DownloadJobFileList} from "./DownloadJobFileList";


export const DownloadSingle = () => {
    const navigate = useNavigate();
    const {downloadId} = useParams() as {downloadId: string};
    const {data: download, refresh} = useBulkDownload(downloadId);
    const {mutation} = useBulkDownloads();
    const repeatDownload = async () => {
        const confirmation = window.confirm("Are you sure you want to repeat this bulk download?");
        
        if (confirmation && download) {
            mutation.mutateAsync({
                command: "post", bulkDownloads: [
                    {
                        name: `(DUPLICATE) ${download.name}`,
                        cloudPath: download.cloudPath,
                        localPath: download.localPath,
                        projectId: download.projectId,
                        hubId: download.hubId,
                        apsFolderId: download.apsFolderId
                    }
                ]
            })
                .then(() => {
                    navigate('/downloads')
                })
            
        } //if
    }
    
    function onDelete(){
        const confirmation = window.confirm(`Are you sure you want to delete this download record? \r\n\r\nThis will not remove any files that have already been downloaded.`)
        
        if(confirmation && download){
            mutation.mutateAsync({command: 'delete', bulkDownloads: [download]})
                .then(() => {
                    navigate('/downloads')
                })
        }
    }
    
    const date = new Date(download?.createdAt ?? 0);
    const sum = download?.files?.reduce((a, b) => a + (b.fileSize || 0), 0);
    const avg = sum ? sum / (download?.files?.length ?? 1) : 0;
    
    return (
        <>
            <BulkDownloadSingleHeader bulkDownload={download!} refresh={refresh} />
            
            <Box sx={{display: "flex", gap: 1, mb: 1}}>
                <Button onClick={() =>  repeatDownload()} variant={'contained'} color={"success"}>Repeat Download</Button>
                <Button onClick={onDelete} variant={"outlined"} color={'error'}>Delete Download Record</Button>
            </Box>
            
            <Box sx={{width: "100%",display: "flex", gap:1}}>
                <Box sx={{width: "50%", float: "left"}}>
                    <h4>Local Path: {download?.localPath}</h4>
                    <h4>Created At: {download?.createdAt ? date.toLocaleString() : "Unknown"}</h4>
                    <h4>File Count: {download?.files?.length}</h4>
                    <h4>Total Amount Downloaded: {bytesToSize(sum)}</h4>
                    <h4>Avg File Size: {bytesToSize(avg)}</h4>

                    <h4>Logs:</h4>
                    <code>{download?.logs}</code>
                </Box>
                <Box sx={{width: "50%", float: "right"}}>
                    {download?.id ? <DownloadJobFileList download={download} /> : <></>}
                </Box>
            </Box>
            
        </>
    )
}

