import {useNavigate, useParams} from "react-router";
import {useBulkUpload} from "../../Utilities/Hooks/useBulkUpload.ts";
import {IBulkUploadAutodeskMirror, useBulkUploads} from "../../Utilities/Hooks/useBulkUploads.ts";
import Button from "@mui/material/Button";
import {BulkUploadSingleHeader} from "./BulkUploadSingleHeader.tsx";
import {DataGrid, GridColDef} from "@mui/x-data-grid";

export const BulkUploadSingle = () => {
    const navigate = useNavigate();
    const {bulkUploadId} = useParams() as { bulkUploadId: string };
    const {mutation} = useBulkUploads();
    const {data: bulkUpload, refresh} = useBulkUpload(bulkUploadId)

    function onDelete() {
        const confirmation = window.confirm("Are you sure you want to delete this record? \r\n\r\nThis will not remove any files that have already been uploaded.");

        if (confirmation && bulkUpload) {
            mutation.mutateAsync({command: 'delete', bulkUploads: [bulkUpload]})
                .then(() => {
                    navigate('/bulkUploads')
                })
        }
    }

    function repeatUpload(status: "Pending" | "Preview") {
        const confirmation = window.confirm("Are you sure you want to repeat this bulk upload?");

        if (confirmation && bulkUpload) {
            mutation.mutateAsync({
                command: 'post', bulkUploads: [
                    {
                        name: `(${status === "Preview" ? "Preview" : "Duplicate"}) ${bulkUpload.name}`,
                        localPath: bulkUpload.localPath,
                        projectId: bulkUpload.projectId,
                        folderId: bulkUpload.folderId,
                        excludedFolderNames: bulkUpload.excludedFolderNames,
                        excludedFileTypes: bulkUpload.excludedFileTypes,
                        modifyPathScript: bulkUpload.modifyPathScript,
                        useModifyPathScript: bulkUpload.useModifyPathScript,
                        status: status
                    }
                ]
            })
                .then(() => {
                    navigate('/bulkUploads')
                })
        }
    }

    const columns: GridColDef<IBulkUploadAutodeskMirror>[] = [
        {field: 'folderName', headerName: 'Name', flex: 1},
        {field: 'relativeFolderPath', headerName: 'Relative Path', flex: 1, renderCell: ({row, value})=>(
            <a href={row.folderUrl} target={'_blank'}>{value}</a>)}
    ]

    return (
        <>
            <BulkUploadSingleHeader bulkUpload={bulkUpload!} refresh={refresh}/>

            <div style={{display: 'flex', gap: '1em'}}>

                <Button onClick={()=>repeatUpload("Preview")} variant={'contained'} color={'secondary'}>Create Dry Run</Button>
                <Button onClick={()=>repeatUpload("Pending")} variant={'contained'} color={'success'}>Repeat Upload</Button>
                <Button onClick={onDelete} variant={"outlined"} color={'error'}>Delete Upload Record</Button>
            </div>
            <div style={{display: 'flex', gap: '1em'}}>
                <div style={{display: 'flex', flex: 1, flexDirection: 'column', gap: '1em', minWidth: '400px'}}>


                    <h4>Settings:</h4>
                    <div style={{display: 'grid', gridTemplateColumns: 'auto auto', gap: '1em'}}>
                        <label>Local Folder Path:</label>
                        <div>{bulkUpload?.localPath}</div>

                        <label>Excluded File Types:</label>
                        <div>{bulkUpload?.excludedFileTypes}</div>

                        <label>Excluded Folder Names:</label>
                        <div>{bulkUpload?.excludedFolderNames}</div>

                        <label>Modify Path Script:</label>
                        <code>{bulkUpload?.modifyPathScript}</code>
                    </div>

                    <h4>Logs:</h4>
                    <code>{bulkUpload?.logs}</code>
                </div>
                <div style={{display: 'flex', flex: 1, flexDirection: 'column', gap: '1em', minWidth: '400px'}}>

                    <h4>Autodesk Mirror:</h4>
                    <DataGrid
                        columns={columns}
                        rows={bulkUpload?.autodeskMirrors ?? []}
                    />
                </div>
            </div>
        </>
    )
}

