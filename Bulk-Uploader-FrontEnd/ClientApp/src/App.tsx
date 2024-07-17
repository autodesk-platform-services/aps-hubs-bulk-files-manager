import './App.css'
import {useSettings} from "./Utilities/Hooks/useSettings.ts";
import {Navigate, Route, Routes, useLocation} from "react-router";
import {BulkUploadCreate} from "./Pages/BulkUploads/BulkUploadCreate.tsx";
import { SettingsPage } from "./Pages/SettingsPage.tsx";
import { BulkUploadList } from "./Pages/BulkUploads/BulkUploadList.tsx";
import {LoadingPage} from "./Pages/LoadingPage.tsx";
import {Layout} from "./Components/Layout.tsx";
import {BulkUploadSingle} from "./Pages/BulkUploads/BulkUploadSingle.tsx";
import {DownloadSingle} from "./Pages/BulkDownloads/DownloadSingle";
import {BulkUploadSingleStatus} from "./Pages/BulkUploads/BulkUploadSingleStatus.tsx";
import { UtilitiesPage } from './Pages/UtilitiesPage.tsx';
import { SelectionsPage } from './Pages/SelectionsPage.tsx';
import {InitialSetupPage} from "./Pages/InitialSetupPage";
import { BatchList } from './Pages/Batches/BatchLists.tsx';
import {DownloadsCreate} from "./Pages/BulkDownloads/DownloadsCreate";
import {DownloadsList} from "./Pages/BulkDownloads/DownloadsList";
import {DownloadSingleStatus} from "./Pages/BulkDownloads/DownloadSingleStatus";
import {useIsAuthenticated, useThreeLegged} from "./Utilities/Hooks/useThreeLegged.ts";

export default function App() {
    const {dataHash: settingsHash, isLoading} = useSettings();
    const {isLoading: threeLeggedLoading} = useThreeLegged();
    const {isAuthenticated, isLoading: isAuthenticatedLoading} = useIsAuthenticated();

    if (isLoading || threeLeggedLoading) return <LoadingPage/>
    if (!settingsHash['ClientId']?.value || !settingsHash['ClientSecret']?.value) return <InitialSetupPage/>

    return (
        <Routes>
            <Route element={<Layout/>}>
                <Route path={"/bulkUploads"} element={<BulkUploadList/>}/>
                <Route path={"/bulkUploads/create"} element={<BulkUploadCreate/>}/>
                <Route path={"/bulkUploads/:bulkUploadId"} element={<BulkUploadSingle/>}/>
                <Route path={"/downloads/:downloadId"} element={<DownloadSingle/>}/>
                <Route path={"/bulkUploads/:bulkUploadId/status/:status"} element={<BulkUploadSingleStatus/>}/>
                <Route path={"/downloads/create"} element={<DownloadsCreate />}/>
                <Route path={"/downloads/:bulkDownloadId/status/:status"} element={<DownloadSingleStatus/>}/>
                <Route path={"/downloads"} element={<DownloadsList />}/>
                <Route path={"/utilities"} element={<UtilitiesPage/>}/>
                <Route path={"/selections"} element={<SelectionsPage/>}/>
                <Route path={"/settings"} element={<SettingsPage/>}/>
                <Route path={"/batches"} element={<BatchList/>}/>
            </Route>
            <Route path={"*"} element={<Navigate to={"/bulkUploads/create"} replace/>}/>
        </Routes>
    )
}