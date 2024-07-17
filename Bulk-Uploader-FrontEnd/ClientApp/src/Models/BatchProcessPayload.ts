export class BatchProcessPayload {
    

    constructor (dbId: number = 0, batchType: string= "", action: string = "", buttonLabel: string = "")
    {
        this.dbId = dbId;
        this.batchType = batchType;
        this.buttonLabel = buttonLabel;
        this.action = action;
    }
    
    dbId: number;
    batchType: string ;
    action: string;
    buttonLabel: string ;
}

