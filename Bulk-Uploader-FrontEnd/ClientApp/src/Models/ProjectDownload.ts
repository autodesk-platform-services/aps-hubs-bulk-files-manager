export class ProjectDownload {
    constructor (sourceHub: string, sourceProject: string, downloadFolder: string, includeProject: boolean, ignoreExtensions: string, ignoreFolders: string)
    {
        this.sourceHub = sourceHub;
        this.sourceProject = sourceProject;
        this.downloadFolder = downloadFolder;
        this.includeProject = includeProject;
        this.ignoreExtensions = ignoreExtensions;
        this.ignoreFolders = ignoreFolders;
    }

    sourceHub: string;
    sourceProject: string;
    includeProject: boolean;
    downloadFolder: string;
    ignoreExtensions: string | null;
    ignoreFolders: string | null;
}