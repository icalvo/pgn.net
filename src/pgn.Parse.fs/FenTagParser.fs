[<AutoOpen>]
module internal ilf.pgn.PgnParsers.FenTagParser

open FParsec
open ilf.pgn.Data
open System.Linq

let pFenPieces = 
        (pchar 'p' >>% [Some (Black, Pawn)])
    <|> (pchar 'n' >>% [Some (Black, Knight)])
    <|> (pchar 'b' >>% [Some (Black, Bishop)])
    <|> (pchar 'r' >>% [Some (Black, Rook)])
    <|> (pchar 'q' >>% [Some (Black, Queen)])
    <|> (pchar 'k' >>% [Some (Black, King)])
    <|> (pchar 'P' >>% [Some (White, Pawn)])
    <|> (pchar 'N' >>% [Some (White, Knight)])
    <|> (pchar 'B' >>% [Some (White, Bishop)])
    <|> (pchar 'R' >>% [Some (White, Rook)])
    <|> (pchar 'Q' >>% [Some (White, Queen)])
    <|> (pchar 'K' >>% [Some (White, King)])
    <|> (pint32 |>> fun n -> Enumerable.Repeat(None, n) |> List.ofSeq)

let check8elem (msg: string) (row: 'a list) : Parser<_, _> =
    fun stream ->
        match row.Length with 
        | 8 -> Reply(row) 
        | _  -> Reply(Error, messageError(msg))

let pFenRow = 
    many pFenPieces |>> fun lists -> List.concat lists
    >>= check8elem "Invalid fen row length. Rows must be of length 8"


let checkBoardLenght (row: 'a list) : Parser<_, _> =
    fun stream ->
        match row.Length with 
        | 8 -> Reply(row) 
        | _  -> Reply(Error, messageError(sprintf "Invalid fen row lenght (%d). Rows must be of length 8"  row.Length ))

let pPiecePositions =
    sepEndBy1 pFenRow (pchar '/') >>= check8elem "Invalid fen row count. There must be 8 rows."
    |>> fun lists -> List.concat lists
    
let pFenCastlingInfo =
    attempt(pchar '-' >>% [ false; false; false; false] <!> "noCastling")
    <|> (
        (attempt(pchar 'K' >>% true <!> "king side white") <|> preturn false) .>>.
        (attempt(pchar 'Q' >>% true) <|> preturn false) .>>.
        (attempt(pchar 'k' >>% true) <|> preturn false) .>>.
        (attempt(pchar 'q' >>% true) <|> preturn false)
        |>> fun(((K, Q), k), q) -> [K; Q; k; q]
    )

let pFenEnPassantSquare = 
    attempt(pchar '-' >>% None)
    <|> (pFile .>>. pRank |>> fun (f, r) -> Some (f, r))

let pFenTagValue = 
    pchar '"' >>. pPiecePositions .>> ws
    .>>. ((pchar 'w' >>% true) <|> (pchar 'b' >>% false)) .>> ws
    .>>. pFenCastlingInfo .>> ws
    .>>. pFenEnPassantSquare .>> ws
    .>>. pint32 .>> ws
    .>>. pint32 .>> ws 
    .>> pchar '"'
    |>> fun (((((pieces, whiteMove), castlingInfo), enPassantSquare), halfMoves), fullMoves) -> 
            //for i in 0 .. 63 do
            //    boardSetup.[i] <- pieces.[i]
            
            let boardSetup  = {
                Board = Array2D.create 8 8 None
                IsWhiteMove = whiteMove;
                CanWhiteCastleKingSide = castlingInfo.[0];
                CanWhiteCastleQueenSide = castlingInfo.[1];
                CanBlackCastleKingSide = castlingInfo.[2];
                CanBlackCastleQueenSide = castlingInfo.[3];
                EnPassantSquare = enPassantSquare;
                HalfMoveClock = halfMoves;
                FullMoveCount = fullMoves
            }

            FenTag("FEN", boardSetup) :> PgnTag;

    <!> "pFenTagValue"