import Button from "@mui/material/Button";
import { BatchJob } from "../Models/BatchJob";
import { BatchProcessPayload } from "../Models/BatchProcessPayload";
import axios, { Axios } from "axios";


export function BatchStatus({ batchJob }: {
    batchJob: BatchJob;

}) {
    const payload = new BatchProcessPayload(batchJob.id, batchJob.type, "queued", "Queue Jobs");
    let btncolor: "primary" | "info" | "secondary" = "primary";
    let complete = false;
    if (batchJob.queued == null){
        //
    } else if (batchJob.started === null) {
        payload.buttonLabel= "Start";
        payload.action = "start";
        btncolor = "secondary";
    } else if (batchJob.completed !== null) {
        payload.buttonLabel = "";
        payload.action = "";
        btncolor = "info";
    } 
    return (
        <div style={{ display: "flex", justifyContent: "flexend", gap: "6px" }}>
            {(!complete) ?
                <Button
                    title={payload.buttonLabel}
                    sx={{ flex: "1", color: btncolor }}
                    color={btncolor}
                    variant={'outlined'}
                    onClick={(e) => e.stopPropagation()}
                >
                    {payload.buttonLabel}
                </Button> : null}
            <Button
                title="Details"
                sx={{ flex: "1" }}
                variant={'outlined'}


                onClick={(e) =>{
                    axios.post("api/ttt",{test:"ttest"})
                        .then((response: any) => {
                            // create file link in browser's memory
                            const t = response;
                        })
                        .catch((e) => {
                            console.error(e);
                        });
                }}
            >
                Details
            </Button>
        </div>
    );
}
