import {useMutation} from "@tanstack/react-query";
import {SimplePost} from "../SimpleREST.ts";
import {IBulkUpload, IBulkUploadFile} from "./useBulkUploads.ts";

export function useScripting(){

    const mutation = useMutation({
        mutationFn: ({command, bulkUpload, bulkUploadFile}: {command: 'post', bulkUpload: Partial<IBulkUpload>, bulkUploadFile: Partial<IBulkUploadFile>})=>{
            switch(command){
                case 'post': return SimplePost<IBulkUploadFile>(`/api/scripting/modifyPath`, {
                    bulkUpload,
                    bulkUploadFile
                });
            }
        }
    })
    return {mutation}
}