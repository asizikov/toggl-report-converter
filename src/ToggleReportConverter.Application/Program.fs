open System
open System.IO
open FSharp.Data

type Record = {Task:String; Date:DateTime}
type Day = {DayOfMonth:int; Tasks:Set<String>}
type TogglEntity = CsvProvider<"User,Email,Client,Project,Task,Description,Billable,Start date,Start time,EndDate,End time,Duration,Tags,Amount ()", HasHeaders=true>

let toDate x = DateTime.Parse x
let toDayOfMonth (x:DateTime) = x.Day

let toTaskSet (r:seq<Record>) = r |> Seq.map (fun s -> s.Task) |> Set.ofSeq
let toRecord (input:TogglEntity.Row) = {Task = input.Description; Date = toDate input.EndDate}
let toCsvLine i = sprintf "%d, \"%s\"" i.DayOfMonth (i.Tasks |> String.concat ", ")
let fileName dir = sprintf "%s/%s.csv" dir (DateTime.Now.AddMonths(-1).ToString("Y"))

let readFile path =
    if not <| File.Exists path then
        printfn "Input file does not exist. %s" path
        None
    else
        Some(TogglEntity.Load(path).Rows)

let writeFile dir data =
    if Directory.Exists dir then
        File.WriteAllLines(fileName dir, data) |> ignore
    else
        printf "Failed to find output directory %s" dir

[<EntryPoint>]
let main argv =
    let input = argv.[0]
    let out = argv.[1]
    match readFile input with
    | None -> Console.ReadKey |> ignore
    | Some lines -> lines
                     |> Seq.map toRecord
                     |> Seq.groupBy (fun r -> r.Date)
                     |> Seq.sortBy fst
                     |> Seq.map(fun gr -> {DayOfMonth = toDayOfMonth(fst gr); Tasks = toTaskSet(snd gr)} )
                     |> Seq.map toCsvLine
                     |> Seq.toArray
                     |> writeFile out
    printfn "Report Completed"
    Console.ReadKey() |> ignore;
    0
