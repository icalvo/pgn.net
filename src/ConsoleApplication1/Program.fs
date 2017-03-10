open ilf.pgn;
open ilf.pgn.Data;
open System.Linq;

type Color = White | Black

let playerColor (game: Game) (player: string) =
    match (game.WhitePlayer, game.BlackPlayer) with
    | (w, b) when w = player -> White
    | (w, b) when b = player -> Black
    | _ -> failwith "player did not play here"

let opponent (game: Game) (player: string) =
    match (game.WhitePlayer, game.BlackPlayer) with
    | (w, b) when w = player -> b
    | (w, b) when b = player -> w
    | _ -> failwith "player did not play here"

let score (game: Game) (player: string) =
    let playerCol = playerColor game player
    match game.Result with
    | GameResult.Open -> 0
    | GameResult.White -> if playerCol = White then 1 else -1
    | GameResult.Black -> if playerCol = Black then 1 else -1
    | GameResult.Draw -> 0
    | _ -> failwith "unexpected game result"

[<EntryPoint>]
let main argv = 
    let reader = new PgnReader()
    let gameDb = reader.ReadFromFile(@"C:\Users\ignac_000\Downloads\chess_com_games_2017-02-26.pgn")

    let query = query {
        for game in gameDb.Games do
        let opponent = opponent game "ignaciocalvo"
        let score = score game "ignaciocalvo"
        groupBy opponent into g
        where (g.Count() > 1)
        let total = g.Sum(fun (g, o, s) -> s)
        sortByDescending (g.Count())
        select (g.Key, g.Count(), total)
    }

    query
    |> Seq.iter (printfn "%A")
    0
