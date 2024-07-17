

export const SimpleDate = ({date}: {date: string})=>{
    if(!date) return <>---</>
    const dateString = new Date(date)?.toLocaleString();
    return (<div title={dateString}>
        {dateString}
    </div>)
}

export const SimpleDate2 = ({date}: {date: string})=>{
    if(!date) return <>---</>
    const d = new Date(date);
    const  dstr = [
      
      ('0' + (d.getMonth() + 1)).slice(-2),
      ('0' + d.getDate()).slice(-2),
      d.getFullYear().toString().slice(-2)
    ].join('.') + " " + [
        d.getHours().toString().slice(-2),
        ('0' + d.getMinutes()).slice(-2)
      ].join(':')

    
    return (<div title={dstr}> 
        {dstr}
    </div>)
}