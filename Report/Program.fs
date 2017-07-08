open System
open System.IO

type Record = {Task:String; Date:DateTime}
type Day = {DayOfMonth:int; Tasks:Set<String>}

let readLines filePath = File.ReadLines(filePath)
let split (input:string) = input.Split(',')
let toDate x = DateTime.Parse x
let toDayOfMonth (x:DateTime) = x.Day

let toTaskSet (r:seq<Record>) = r |> Seq.map (fun s -> s.Task) |> Set.ofSeq
let toRecord (input:array<String>) = {Task = input.[5]; Date = toDate input.[7]}
let toCsvLine i = sprintf "%d, \"%s\"" i.DayOfMonth (i.Tasks |> String.concat ", ")
let fileName dir =  sprintf "%s/%s.csv" dir (DateTime.Now.AddMonths(-1).ToString("Y"))

[<EntryPoint>]
let main argv = 
    let input = argv.[0]

    let records = readLines input
                     |> Seq.skip 1
                     |> Seq.map split
                     |> Seq.map toRecord
                     |> Seq.groupBy (fun r -> r.Date)
                     |> Seq.sortBy fst
                     |> Seq.map(fun gr -> {DayOfMonth = toDayOfMonth(fst gr); Tasks = toTaskSet(snd gr)} )
                     |> Seq.map toCsvLine
    File.WriteAllLines(fileName argv.[1], records)
    Console.ReadKey() |> ignore;
    0