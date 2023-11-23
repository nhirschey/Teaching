#r "nuget: FSharp.Data, 5.0.2"
#r "nuget: FSharp.Collections.ParallelSeq"

#time "on"

open System
open FSharp.Data
open FSharp.Data.CsvExtensions
open FSharp.Collections.ParallelSeq

(*
type FullCsv = CsvProvider<"c:/users/nicho/downloads/usa2000.csv",
                           MissingValues=".",
                           PreferOptionals=true,
                           InferRows=10_000>

let full = FullCsv.GetSample()
full.Rows |> Seq.item 1
full.Headers.Value.[24..26]
let x = full.Rows |> Seq.item 1

full.Rows
|> Seq.countBy(fun x -> x.Permno.IsSome)
*)

let full = CsvFile.Load("c:/users/nicho/downloads/usa2000.csv")
let fullRows = full.Rows |> Seq.toArray


let columns =
    [
        // Identifiers
        "id(string)"   
        "eom(date)"
        "sizeGrp(string)"
        "obsMain(bool)"
        "exchMain(bool)"
        "primarySec(bool)"
        "permno(int Option)"
        "excntry(string)"
        "common(bool)"
        "crspShrcd(int Option)"
        "crspExchcd(int Option)"
        "adjfct(float Option)"
        "shares(float Option)"
        "gics(int Option)"
        "ff49(int Option)"
        // Market variables
        "ret(float Option)"
        "retExc(float Option)"
        "prc(float Option)"
        "marketEquity(float Option)"
    ]

//String.concat "," columns
let [<Literal>] Schema = 
    "id(string),eom(date),source(string),sizeGrp(string),obsMain(bool),exchMain(bool),primarySec(bool),gvkey(string),iid(string),"+
    "permno(int Option),permco(int Option),excntry(string),curcd(string),fx(string),common(bool),compTpci(string),crspShrcd(int Option),compExchg(string),crsp_exchcd(int Option),adjfct(float Option),shares(float Option),gics(int Option),sic(int Option),naics(int Option),ff49(int Option),ret(float Option),retExc(float Option),prc(float Option),marketEquity(float Option)"

type IdVarsCsv = CsvProvider<Sample=Schema,Schema=Schema>
let intOption x =
    match x with
    | "" -> None
    | s -> Some (x.AsInteger())
let floatOption x =
    match x with
    | "" -> None
    | s -> Some (x.AsFloat())
let dateParse x = 
    DateTime.ParseExact(x,
        "yyyyMMdd",
        Globalization.CultureInfo.InvariantCulture)

let makeIdVarsRow row =
    IdVarsCsv.Row(
        id=row?id,   
        eom=dateParse row?eom,
        source=row?source,
        sizeGrp=row?size_grp,
        obsMain=row?obs_main.AsBoolean(),
        exchMain=row?exch_main.AsBoolean(),
        primarySec=row?primary_sec.AsBoolean(),
        gvkey=row?gvkey,
        iid=row?iid,
        permno=intOption row?permno,
        permco=intOption row?permco,
        excntry=row?excntry,
        curcd=row?curcd,
        fx=row?fx,
        common=row?common.AsBoolean(),
        compTpci=row?comp_tpci,
        crspShrcd=intOption row?crsp_shrcd,
        compExchg=row?comp_exchg,
        crspExchcd=intOption row?crsp_exchcd,
        adjfct=floatOption row?adjfct,
        shares=floatOption row?shares,
        gics=intOption row?gics,
        sic=intOption row?sic,
        naics=intOption row?naics,
        ff49=intOption row?ff49,
        ret=floatOption row?ret,
        retExc=floatOption row?ret_exc,
        prc = floatOption row?prc,
        marketEquity = floatOption row?market_equity)
        

let signals =
    [|
        "niq_su"
        "ret_6_1"
        "ret_12_1"
        "saleq_su"
        "tax_gr1a"
        "ni_inc8q"
        "prc_highprc_252d"
        "resff3_6_1"
        "resff3_12_1"
        "be_me"
        "debt_me"
        "at_me"
        "ret_60_12"
        "ni_me"
        "fcf_me"
        "div12m_me"
        "eqpo_me"
        "eqnpo_me"
        "sale_gr3"
        "sale_gr1"
        "ebitda_mev"
        "sale_me"
        "ocf_me"
        "intrinsic_value"
        "bev_mev"
        "netdebt_me"
        "eq_dur"
        "capex_abn"
        "at_gr1"
        "ppeinv_gr1a"
        "noa_at"
        "noa_gr1a"
        "lnoa_gr1a"
        "capx_gr1"
        "capx_gr2"
        "capx_gr3"
        "chcsho_12m"
        "eqnpo_12m"
        "debt_gr3"
        "inv_gr1"
        "inv_gr1a"
        "oaccruals_at"
        "taccruals_at"
        "cowc_gr1a"
        "coa_gr1a"
        "col_gr1a"
        "nncoa_gr1a"
        "ncoa_gr1a"
        "ncol_gr1a"
        "nfna_gr1a"
        "sti_gr1a"
        "lti_gr1a"
        "fnl_gr1a"
        "be_gr1a"
        "oaccruals_ni"
        "taccruals_ni"
        "netis_at"
        "eqnetis_at"
        "dbnetis_at"
        "niq_be"
        "niq_be_chg1"
        "niq_at"
        "niq_at_chg1"
        "ebit_bev"
        "ebit_sale"
        "sale_bev"
        "at_turnover"
        "gp_at"
        "gp_atl1"
        "ope_be"
        "ope_bel1"
        "op_at"
        "op_atl1"
        "cop_at"
        "cop_atl1"
        "f_score"
        "o_score"
        "z_score"
        "pi_nix"
        "at_be"
        "saleq_gr1"
        "rd_me"
        "rd_sale"
        "opex_at"
        "emp_gr1"
        "rd5_at"
        "age"
        "dsale_dinv"
        "dsale_drec"
        "dgp_dsale"
        "dsale_dsga"
        "sale_emp_gr1"
        "tangibility"
        "kz_index"
        "ocfq_saleq_std"
        "cash_at"
        "ni_ar1"
        "ni_ivol"
        "earnings_variability"
        "aliq_at"
        "aliq_mat"
        "seas_1_1an"
        "seas_1_1na"
        "seas_2_5an"
        "seas_2_5na"
        "seas_6_10an"
        "seas_6_10na"
        "seas_11_15an"
        "seas_11_15na"
        "seas_16_20an"
        "seas_16_20na"
        "market_equity"
        "ivol_ff3_21d"
        "ivol_capm_252d"
        "ivol_capm_21d"
        "ivol_hxz4_21d"
        "rvol_21d"
        "beta_60m"
        "betabab_1260d"
        "beta_dimson_21d"
        "turnover_126d"
        "turnover_var_126d"
        "dolvol_126d"
        "dolvol_var_126d"
        "prc"
        "ami_126d"
        "zero_trades_21d"
        "zero_trades_126d"
        "zero_trades_252d"
        "rmax1_21d"
        "rskew_21d"
        "iskew_capm_21d"
        "iskew_ff3_21d"
        "iskew_hxz4_21d"
        "coskew_21d"
        "ret_1_0"
        "betadown_252d"
        "bidaskhl_21d"
        "ret_3_1"
        "ret_9_1"
        "ret_12_7"
        "corr_1260d"
        "rmax5_21d"
        "rmax5_rvol_21d"
        "ni_be"
        "ocf_at"
        "ocf_at_chg1"
        "mispricing_perf"
        "mispricing_mgmt"
        "qmj"
        "qmj_prof"
        "qmj_growth"
        "qmj_safety"
    |]

type SignalCsv = CsvProvider<"id(string),eom(date),signal(float option)">
let makeSignalRow (name:string) (row:CsvRow) =
    SignalCsv.Row(eom=dateParse row?eom,
                  id=row?id,
                  signal=floatOption row.[name])
let makeSignal (input:array<CsvRow>) name =
    name,
    input
    |> Array.map (makeSignalRow name)
    |> fun rows -> new SignalCsv(rows)
let saveSignal (directory:string) (signalName:string,signalFile:SignalCsv) =
    IO.Directory.CreateDirectory(directory) |> ignore
    signalFile.Save($"{directory}/{signalName}.csv")

open System
open System.IO
open System.IO.Compression

let saveSignalZip (directory:string) (signalName:string,signalFile:SignalCsv) =
    IO.Directory.CreateDirectory(directory) |> ignore
    use zipToOpen = new FileStream($"{directory}/{signalName}.zip",FileMode.OpenOrCreate)
    use archive = new ZipArchive(zipToOpen,ZipArchiveMode.Update)
    let entry = archive.CreateEntry($"{signalName}.csv")
    use writer = new StreamWriter(entry.Open())
    signalFile.Save(writer)


let output = 
    full.Rows 
    |> PSeq.map makeIdVarsRow 
    |> PSeq.sortBy(fun x -> x.Eom)
    |> fun rows -> new IdVarsCsv(rows)

output.Save(__SOURCE_DIRECTORY__ + "/../data-cache/id_and_return_data.csv")


signals
|> Array.Parallel.map (makeSignal fullRows)
|> Array.iter (saveSignalZip (__SOURCE_DIRECTORY__ + "/../data-cache/signalsDataZips"))
