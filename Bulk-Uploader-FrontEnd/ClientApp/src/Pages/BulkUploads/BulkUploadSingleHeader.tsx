import {IBulkUpload} from "../../Utilities/Hooks/useBulkUploads.ts";
import {NavLink} from "react-router-dom";
import {ColoredStatusButton} from "../../Components/ColoredStatusButton.tsx";
import IconButton from "@mui/material/IconButton";
import SyncIcon from "@mui/icons-material/Sync";
import {LinearProgress} from "@mui/material";

export const BulkUploadSingleHeader = ({bulkUpload, refresh}: { bulkUpload?: IBulkUpload, refresh?: () => void }) => {

    return (
        <>
            {bulkUpload ? <div style={{display: 'flex', alignItems: 'center', gap: '1em'}}>
                <h2 style={{display: 'flex', gap: '1em'}}>
                    <NavLink style={{textDecoration: 'none'}} to={'/bulkUploads'}>Bulk Uploads</NavLink>
                    :
                    <NavLink style={{textDecoration: 'none'}}
                             to={`/bulkUploads/${bulkUpload?.id}`}>{bulkUpload?.name}</NavLink>
                </h2>
                <h4>{bulkUpload?.status}</h4>

                <div style={{flex: 1}}/>
                <div>
                    <ColoredStatusButton
                        title={"Proposed File Uploads"}
                        color={'secondary'}
                        count={bulkUpload?.proposedFileCount}
                        path={`/bulkUploads/${bulkUpload.id}/status/Proposed`}
                    />
                    <ColoredStatusButton
                        title={"Skipped File Uploads"}
                        color={'warning'}
                        count={bulkUpload?.doNotUploadFileCount}
                        path={`/bulkUploads/${bulkUpload.id}/status/DoNotUpload`}
                    />
                    <ColoredStatusButton
                        title={"Pending File Uploads"}
                        color={'info'}
                        count={bulkUpload?.pendingFileCount}
                        path={`/bulkUploads/${bulkUpload.id}/status/Pending`}
                    />
                    <ColoredStatusButton
                        title={"Successful File Uploads"}
                        color={'success'}
                        count={bulkUpload?.successFileCount}
                        path={`/bulkUploads/${bulkUpload.id}/status/Success`}
                    />
                    <ColoredStatusButton
                        title={"Failed File Uploads"}
                        color={'error'}
                        count={bulkUpload?.failedFileCount}
                        path={`/bulkUploads/${bulkUpload.id}/status/Failed`}
                    />
                    {refresh && <IconButton onClick={refresh}>
                        <SyncIcon/>
                    </IconButton>}
                </div>
            </div> : <LinearProgress/>}
        </>
    )
}