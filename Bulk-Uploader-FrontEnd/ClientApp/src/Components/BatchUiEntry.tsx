
import { Box, Button, Card } from "@mui/material";
import React, { useState } from "react";
import { BatchEntry } from "../Models/BatchEntry";
import axios from "axios";


export const BatchUiEntry = ({ entry }: { entry: BatchEntry }) => {
    const [selectFile, setSelectFile] = useState();
    const fileRef = React.useRef<any>(null);

    function getMockData() {
        const url = entry.batchTemplateUrl;
        const filename = `${entry.key}.template.xlsx`;
        axios({
            url: url, //your url
            method: "GET",
            responseType: "blob", // important
        })
            .then((response: any) => {
                // create file link in browser's memory
                const href = URL.createObjectURL(response.data);

                // create "a" HTML element with href to file & click
                const link = document.createElement("a");
                link.href = href;
                link.setAttribute("download", filename); //or any other extension
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

    const handleSelection = () => {
        if (fileRef != null && fileRef.current != null)
            fileRef.current.click();
        console.log("refChanged");
    };


    const onSelectedFileChange = async (e: any) => {
        setSelectFile(e.target.files[0]);
        
        const formData = new FormData();
        formData.append("FileData", e.target.files[0]);
        
        try {
            const res = await axios.post(`api/batch/load/${entry.key}`, formData);
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

        <div style={{ height: "55px", display: "flex", flexDirection: "row", gap: "10px" }}>
            <Box
                style={{
                    flex: 3,
                    backgroundColor: 'primary.dark',
                    lineHeight: "55px"

                }}
            >{entry.description}</Box>
            <input
                style={{ display: 'none' }}
                type="file"
                ref={fileRef}
                onChange={onSelectedFileChange}>
            </input>
            <Button
                style={{ flex: "1" }}
                onClick={handleSelection}
                variant="contained"
            >
                Load '{entry.title}' Batch
            </Button>
            <Button
                style={{ flex: "1" }}
                variant="contained"
                onClick={() => {
                    getMockData();
                }}
            >
                Download '{entry.title}' Template
            </Button>
        </div>

    )
}