export interface BatchJob {
    id: number;
    name: string;
    type: string;
    errors: string | null;
    jobCount: number;
    data: string | null;
    created: string | null;
    queued: string | null;
    started: string | null;
    completed: string | null;
}