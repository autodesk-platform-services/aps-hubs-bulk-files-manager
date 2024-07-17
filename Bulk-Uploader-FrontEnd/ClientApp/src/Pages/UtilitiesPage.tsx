import { useSettings } from "../Utilities/Hooks/useSettings.ts";
import React, { useState } from "react";
import { Button, TextField } from "@mui/material";
import axios from "axios";
import { ProjectDownload } from "../Models/ProjectDownload.ts";
import {SimpleGet, SimplePost} from "../Utilities/SimpleREST";
export const UtilitiesPage = () => {
    const {dataHash: settingsHash, mutation} = useSettings();
    const [selectFile, setSelectFile] = useState();
    const fileRef = React.useRef<any>(null);

    const [hubId, setHubId] = useState<string>(
        settingsHash["HubId"]?.value ?? ""
    );
    const [projectId, setProjectId] = useState<string>(
        settingsHash["ProjectId"]?.value ?? ""
    );
    const [projectOutputFolder, setProjectOutputFolder] = useState<string>(
        settingsHash["ProjectOutputFolder"]?.value ?? ""
    );

    function saveChanges() {
        const hubIdSetting = settingsHash["HubId"] ?? {
            key: "HubId",
            value: ""
        };
        const projectIdSetting = settingsHash["ProjectId"] ?? {
            key: "ProjectId",
            value: "",
        };
        const projectOutputFolderSetting = settingsHash["ProjectOutputFolder"] ?? {
            key: "ProjectOutputFolder",
            value: "",
        };

        hubIdSetting.value = hubId;
        projectIdSetting.value = projectId;
        projectOutputFolderSetting.value = projectOutputFolder;

        mutation.mutateAsync({
            command: "patch",
            settings: [hubIdSetting, projectIdSetting, projectOutputFolderSetting],
        });

        //TODO: Verify that this client ID and Secret are valid
    }

    function getHubs() {
        axios({
            url: "api/hubs", //your url
            method: "GET",
            responseType: "blob", // important
        })
            .then((response) => {
                // create file link in browser's memory
                const href = URL.createObjectURL(response.data);

                // create "a" HTML element with href to file & click
                const link = document.createElement("a");
                link.href = href;
                link.setAttribute("download", "hubs.xlsx"); //or any other extension
                document.body.appendChild(link);
                link.click();

                // clean up "a" element & remove ObjectURL
                document.body.removeChild(link);
                URL.revokeObjectURL(href);
            })
            .catch((e) => {
                console.error(e);
            });
    }

    function getProjectsImp(hubId: string) {
        saveChanges();
        const url = `api/hubs/${hubId}/projects`;
        axios({
            url: url, //your url
            method: "GET",
            responseType: "blob", // important
        })
            .then((response) => {
                // create file link in browser's memory
                const href = URL.createObjectURL(response.data);

                // create "a" HTML element with href to file & click
                const link = document.createElement("a");
                link.href = href;
                link.setAttribute("download", `projects.${hubId}.xlsx`); //or any other extension
                document.body.appendChild(link);
                link.click();

                // clean up "a" element & remove ObjectURL
                document.body.removeChild(link);
                URL.revokeObjectURL(href);
            })
            .catch((e) => {
                console.error(e);
            });
    }

    function getContentImp(hubId: string, projectId: string, foldersOnly: boolean) {
        saveChanges();
        const target = foldersOnly ? "folders" : "content";
        const url = `api/hubs/${hubId}/projects/${projectId}/${target}`;
        axios({
            url: url, //your url
            method: "GET",
            responseType: "blob", // important
        })
            .then((response) => {
                // create file link in browser's memory
                const href = URL.createObjectURL(response.data);

                // create "a" HTML element with href to file & click
                const link = document.createElement("a");
                link.href = href;
                link.setAttribute("download", foldersOnly ? `folders.${hubId}/${projectId}.xlsx` : `contents.${hubId}/${projectId}.xlsx`); //or any other extension
                document.body.appendChild(link);
                link.click();

                // clean up "a" element & remove ObjectURL
                document.body.removeChild(link);
                URL.revokeObjectURL(href);
            })
            .catch((e) => {
                console.error(e);
            });
    }

    async function downloadProjectImp() {
        saveChanges();
        const url = `api/bulk/downloadProject?hubId=${hubId}&projectId=${projectId}&downloadFolder=${projectOutputFolder}`;
        await SimplePost<any>(url);
    }
    
    function getProjects() {
        getProjectsImp(hubId);
    }

    async function postDownloads() {
      await downloadProjectImp(); 
    }
    function getFolders() {
        getContentImp(hubId, projectId, true);
    }

    function getContent() {
        getContentImp(hubId, projectId, false);
    }

    const handleSelection = () => {
        if (fileRef != null && fileRef.current != null)
            fileRef.current.click();
        console.log("refChanged");
    };


    const onSelectedFileChange = async (e: any) => {
        setSelectFile(e.target.files[0]);

        const formData = new FormData();
        formData.append("File", e.target.files[0]);

        try {
            const res = await axios.post(`api/downloads/bulk?downloadFolder=${projectOutputFolder}`, formData);
            console.log(res);
        } catch (error) {
            console.log(error)
        }

        // try {
        //     const res = await axios.post(entry.batchLoadUrl, formData);
        //     console.log(res);
        // } catch (error) {
        //     console.log(error)
        // }
    }
    return (
        <>
            <h2>Utilities</h2>

            <div
                style={{
                    padding: "1em",
                    display: "flex",
                    flexDirection: "column",
                    gap: "1em",
                }}
            >
                <div
                    style={{
                        display: "flex",
                        flexDirection: "row",
                        gap: "1em",
                        height: "55px",
                    }}
                >
                    <Button onClick={getHubs} style={{flex: "2"}} variant="contained">
                        Get Hubs (Accounts)
                    </Button>
                </div>

                <div style={{display: "flex", flexDirection: "row", gap: "10px"}}>
                    <TextField
                        style={{flex: "3"}}
                        label={"Hub ID"}
                        value={hubId}
                        onChange={(e) => setHubId(e.currentTarget.value ?? "")}
                    />
                    <Button
                        onClick={getProjects}
                        variant="contained"
                        style={{width: "380px"}}
                    >
                        Get Projects
                    </Button>
                </div>

                <div style={{display: "flex", flexDirection: "row", gap: "10px"}}>
                    <TextField
                        style={{flex: "3"}}
                        label={"Project ID"}
                        value={projectId}
                        onChange={(e) => setProjectId(e.currentTarget.value ?? "")}
                    />

                    <Button
                        style={{width: "186px"}}
                        onClick={getFolders}
                        variant="contained"
                    >
                        Get Folders
                    </Button>
                    <Button
                        style={{width: "185px"}}
                        variant="contained"
                        onClick={getContent}
                    >
                        Get Content
                    </Button>
                </div>

                <div style={{display: "flex", flexDirection: "row", gap: "10px"}}>
                    <TextField
                        style={{flex: "3"}}
                        label={"Download Root Folder"}
                        value={projectOutputFolder}
                        onChange={(e) => setProjectOutputFolder(e.currentTarget.value ?? "")}
                    />
                    <input
                        style={{display: 'none'}}
                        type="file"
                        ref={fileRef}
                        onChange={onSelectedFileChange}>
                    </input>
                    <Button
                        style={{width: "380px"}}
                        onClick={postDownloads}
                        variant="contained"
                    >
                       Download Project Files
                    </Button>
                </div>
            </div>
        </>
    );
}
        

