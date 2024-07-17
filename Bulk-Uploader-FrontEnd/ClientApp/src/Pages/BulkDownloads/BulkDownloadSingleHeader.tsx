import {NavLink} from "react-router-dom";
import {ColoredStatusButton} from "../../Components/ColoredStatusButton.tsx";
import IconButton from "@mui/material/IconButton";
import SyncIcon from "@mui/icons-material/Sync";
import {LinearProgress} from "@mui/material";
import {IBulkDownload} from "../../Utilities/Hooks/useBulkDownloads.ts";

export const BulkDownloadSingleHeader = ({bulkDownload, refresh}: { bulkDownload?: IBulkDownload, refresh?: () => void }) => {
    
    console.log(bulkDownload);

    return (
        <>
            {bulkDownload ? <div style={{display: 'flex', alignItems: 'center', gap: '1em'}}>
                <h2 style={{display: 'flex', gap: '1em'}}>
                    <NavLink style={{textDecoration: 'none'}} to={'/downloads'}>Bulk Downloads</NavLink>
                    :
                    {bulkDownload?.name}
                </h2>
                <h4>{bulkDownload?.status}</h4>

                <div style={{flex: 1}}/>
                <div>
                    <ColoredStatusButton
                        title={"Pending File Downloads"}
                        color={'info'}
                        count={bulkDownload?.pendingDownloadCount}
                        path={`/downloads/${bulkDownload.id}/status/Pending`}
                    />
                    <ColoredStatusButton
                        color={'secondary'}
                        count={bulkDownload?.inProgressDownloadCount}
                        path={`/downloads/${bulkDownload.id}/status/InProgress`}
                    />
                    <ColoredStatusButton
                        title={"Successful File Downloads"}
                        color={'success'}
                        count={bulkDownload?.successDownloadCount}
                        path={`/downloads/${bulkDownload.id}/status/Success`}
                    />
                    <ColoredStatusButton
                        title={"Failed File Downloads"}
                        color={'error'}
                        count={bulkDownload?.failedDownloadCount}
                        path={`/downloads/${bulkDownload.id}/status/Failed`}
                    />
                    {refresh && <IconButton onClick={refresh}>
                        <SyncIcon/>
                    </IconButton>}
                </div>
            </div> : <LinearProgress/>}
        </>
    )
}