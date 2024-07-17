import {useSettings} from "../Utilities/Hooks/useSettings.ts";
import {useState} from "react";
import {Button, TextField} from "@mui/material";
import {useBulkUploadPresets} from "../Utilities/Hooks/useBulkUploadPresets";
import {useThreeLegged} from "../Utilities/Hooks/useThreeLegged.ts";
import {AuthenticationButton} from "../Components/AuthenticationButton.tsx";

export const SettingsPage = () => {
    const {dataHash: settingsHash, mutation} = useSettings();
    const {data: presets} = useBulkUploadPresets({});
    const {refresh} = useThreeLegged();

    const [clientId, setClientId] = useState<string>(settingsHash["ClientId"]?.value ?? "");
    const [clientSecret, setClientSecret] = useState<string>(settingsHash["ClientSecret"]?.value ?? "");
    const [outputFolder, setOutputFolder] = useState<string>(
        settingsHash["OutputFolder"]?.value ?? ""
    );


    const exportSettings = () => {

        const t1 = {
            "Settings": {
                ...settingsHash
            },
            presets,
        }
        const str = JSON.stringify(t1, null, 2);

        const blob = new Blob([str], {
            type: 'application/json'
        });
        const element = document.createElement("a");
        element.href = URL.createObjectURL(blob);
        element.download = "bulk-file-manager-settings.json";
        document.body.appendChild(element);
        element.click();
    }

    function saveChanges() {
        const clientIdSetting = settingsHash["ClientId"] ?? {
            key: "ClientId",
            value: "",
        };
        const clientSecretSetting = settingsHash["ClientSecret"] ?? {
            key: "ClientSecret",
            value: "",
        };
        const outputFolderSetting = settingsHash["OutputFolder"] ?? {
            key: "OutputFolder",
            value: "",
        };

        clientIdSetting.value = clientId;
        clientSecretSetting.value = clientSecret;
        outputFolderSetting.value = outputFolder;

        mutation.mutateAsync({
            command: "patch",
            settings: [
                clientIdSetting,
                clientSecretSetting,
                outputFolderSetting,
            ],
        })
            .then(()=>refresh());

    }

    const isSaveDisabled = !clientId || !clientSecret;


    return (
        <>
            <h2>Settings</h2>

            <div
                style={{
                    padding: "1em",
                    display: "flex",
                    flexDirection: "column",
                    gap: "1em",
                }}
            >
                <TextField
                    label={"Autodesk Client ID"}
                    value={clientId}
                    onChange={(e) => setClientId(e.currentTarget.value ?? "")}
                />

                <TextField
                    label={"Autodesk Client Secret"}
                    value={clientSecret}
                    type={"password"}
                    onChange={(e) => setClientSecret(e.currentTarget.value ?? "")}
                />

                <div>
                    <AuthenticationButton/>
                </div>

                <TextField
                    disabled={false}
                    label={"Output Folder"}
                    value={outputFolder}
                    onChange={(e) => setOutputFolder(e.currentTarget.value ?? "")}
                />

                <Button
                    style={{height: "55px"}}
                    variant={isSaveDisabled ? undefined : "contained"}
                    disabled={isSaveDisabled}
                    onClick={saveChanges}
                >
                    Save Changes
                </Button>


                <Button sx={{mr: 1}}
                        variant={"outlined"}
                        disabled={isSaveDisabled}
                        onClick={() => exportSettings()}
                >
                    Export
                </Button>
            </div>
        </>
    );
};
