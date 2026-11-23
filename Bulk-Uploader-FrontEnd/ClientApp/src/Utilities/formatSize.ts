export function formatSize(bytes?: number): string {
    if (typeof bytes !== "number") return "";
    const units = ["B", "KB", "MB", "GB", "TB"];
    let i = 0;
    let n = bytes;
    while (n >= 1024 && i < units.length - 1) {
        n /= 1024;
        i++;
    }
    return `${i === 0 ? n : n.toFixed(2)} ${units[i]}`;
}
