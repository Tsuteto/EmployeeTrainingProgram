using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace EmployeeTraining.Localization
{
    public class LocalizationSerializer
	{
		private readonly string folderPath = Application.dataPath + "/CashierTraining";
		private readonly string fullFileName;
		private readonly Settings settings;

		public LocalizationSerializer(Plugin plugin)
		{
			this.settings = plugin.Settings;
			this.fullFileName = folderPath + "/Localization-2.0.0.json";
		}

		public StringLocalizer Load()
		{
			List<Localizations> list = null;
			if (this.settings.CustomizeLocalization)
			{
				if (!Directory.Exists(this.folderPath))
				{
					Directory.CreateDirectory(this.folderPath);
					list = this.Default();
				}
				if (File.Exists(this.fullFileName))
				{
					string value = File.ReadAllText(this.fullFileName);
					new JsonSerializerSettings();
					list = JsonConvert.DeserializeObject<List<Localizations>>(value);
				}
			}
			return new StringLocalizer(list ?? this.Default());
		}

		public void Write(List<Localizations> properties)
		{
			string contents = JsonConvert.SerializeObject(properties, Formatting.Indented);
			File.WriteAllText(this.fullFileName, contents);
		}

		public List<Localizations> Default()
		{
			List<Localizations> langs = new List<Localizations>() {
				new Localizations("en") {
					["Cashier Name"] = "Cashier {0}",
					["Restocker Name"] = "Restocker {0}",
					["Customer Helper Name"] = "Customer Helper {0}",
					["Security Guard Name"] = "Security Guard {0}",
					["Janitor Name"] = "Janitor {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} Exp",
					["TRAINING"] = "TRAINING",
					["Cashiers"] = "Cashiers",
					["Restockers"] = "Restockers",
					["Customer Helpers"] = "<size=80%>Customer\nHelpers</size>",
					["Security Guards"] = "<size=80%>Security\nGuards</size>",
					["Janitors"] = "Janitors",
					["Exp."] = "Exp.",
					["Level"] = "Level {0} - {1}",
					["Scans Per Minute"] = "Scans per Minute",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Payment Time",
					["Payment Time Sec."] = "{0} sec.",
					["Daily Wage"] = "Daily Wage",
					["Rapidity"] = "Rapidity",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacity",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Dexterity",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Mastery Roadmap",
					["Rookie"] = "Rookie",
					["Middle"] = "Middle",
					["Advance"] = "Advanced",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Locked",
					["Approve"] = "Approve",
					["Defer"] = "Defer",
					["Train to Level Up"] = "Train to Level Up",
					["Unlock Higher Grade"] = "Unlock Higher Grade",
					["Show head gauge"] = "Show the gauge overhead",
					["Upgrade warning"] = "Would you promote {0}\nto the {1} grade?\n\nIt will be even more efficient but\nits daily wage will INCREASE to {2}</size>.",
					["NO CASHIERS HIRED"] = "NO CASHIERS HIRED",
					["NO RESTOCKERS HIRED"] = "NO RESTOCKERS HIRED",
					["NO CUSTOMER HELPERS HIRED"] = "NO CUSTOMER HELPERS HIRED",
					["NO SECURITY GUARDS HIRED"] = "NO SECURITY GUARDS HIRED",
					["NO JANITORS HIRED"] = "NO JANITORS HIRED",
				},
				new Localizations("ja-JP") {
					["Cashier Name"] = "レジ係 {0}",
					["Restocker Name"] = "補充係 {0}",
					["Customer Helper Name"] = "お客様対応係 {0}",
					["Security Guard Name"] = "警備員 {0}",
					["Janitor Name"] = "清掃係 {0}",
					["Lvl"] = "<size=70%>Lv </size>{0}",
					["Popup Exp"] = "{0} EXP",
					["TRAINING"] = "トレーニング",
					["Cashiers"] = "レジ係",
					["Restockers"] = "補充係",
					["Customer Helpers"] = "<size=95%>お客様対応係</size>",
					["Security Guards"] = "警備員",
					["Janitors"] = "清掃係",
					["Exp."] = "EXP",
					["Level"] = "レベル {0} 【{1}】",
					["Scans Per Minute"] = "スキャン数毎分",
					["SPM Range"] = "{0} ～ {1}",
					["Payment Time"] = "支払い時間",
					["Payment Time Sec."] = "{0} 秒",
					["Daily Wage"] = "日給",
					["Rapidity"] = "素早さ",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "積載量",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "器用さ",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "上達ロードマップ",
					["Rookie"] = "初級",
					["Middle"] = "中級",
					["Advance"] = "上級",
					["Pro"] = "プロ",
					["?"] = "？",
					["Ninja"] = "ニンジャ",
					["Locked"] = "未解放",
					["Train to Level Up"] = "レベルアップまで訓練",
					["Unlock Higher Grade"] = "上位グレードを解放",
					["Approve"] = "承認",
					["Defer"] = "見送る",
					["Show head gauge"] = "頭上にゲージを表示する",
					["Upgrade warning"] = "{0} を『{1}グレード』に昇進させますか？\n\n手際は良くなりますが日給が {2}</size> に上がります。",
					["NO CASHIERS HIRED"] = "レジ係を雇っていません",
					["NO RESTOCKERS HIRED"] = "補充係を雇っていません",
					["NO CUSTOMER HELPERS HIRED"] = "お客様対応係を雇っていません",
					["NO SECURITY GUARDS HIRED"] = "警備員を雇っていません",
					["NO JANITORS HIRED"] = "清掃係を雇っていません",
				},
				new Localizations("it") { // イタリア
					["Cashier Name"] = "Cassiere {0}",
					["Restocker Name"] = "Rifornitore {0}",
					["Customer Helper Name"] = "Assistente clienti {0}",
					["Security Guard Name"] = "Guardia di sicurezza {0}",
					["Janitor Name"] = "Custode {0}",
					["Lvl"] = "<size=70%>Liv </size>{0}",
					["Popup Exp"] = "{0} PE",
					["TRAINING"] = "FORMAZIONE",
					["Cashiers"] = "Cassieri",
					["Restockers"] = "Rifornitori",
					["Customer Helpers"] = "<size=80%>Assistente\nclienti</size>",
					["Security Guards"] = "<size=80%>Guardie di\nsicurezza</size>",
					["Janitors"] = "Custodi",
					["Exp."] = "PE",
					["Level"] = "Livello {0} - {1}",
					["Scans Per Minute"] = "Scansioni al minuto",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Tempo di pagamento",
					["Payment Time Sec."] = "{0} sec.",
					["Daily Wage"] = "Salario giornaliero",
					["Rapidity"] = "Rapidità",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacità",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Destrezza",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Tabella di marcia della maestria",
					["Rookie"] = "Principiante",
					["Middle"] = "Medio",
					["Advance"] = "Avanzato",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Bloccato",
					["Approve"] = "Approvare",
					["Defer"] = "Rimandalo",
					["Train to Level Up"] = "Treno per salire di liv.",
					["Unlock Higher Grade"] = "Sblocca il grado superiore",
					["Show head gauge"] = "Mostrare l'indicatore sulla testa",
					["Upgrade warning"] = "Promuovereste il {0} al grado {1}?\n\nSarà ancora più efficiente ma\nil suo stipendio giornaliero aumenterà a {2}</size>.",
					["NO CASHIERS HIRED"] = "NESSUN CASSIERE ASSUNTO",
					["NO RESTOCKERS HIRED"] = "NESSUN RIFORNITORE ASSUNTO",
					["NO CUSTOMER HELPERS HIRED"] = "NESSUN ASSISTENTE CLIENTI ASSUNTO",
					["NO SECURITY GUARDS HIRED"] = "NESSUN GUARDIE DI SICUREZZA ASSUNTA",
					["NO JANITORS HIRED"] = "NESSUN CUSTODI ASSUNTO",
				},
				new Localizations("fr-FR") { // フランス
					["Cashier Name"] = "Caissier {0}",
					["Restocker Name"] = "Restocker {0}",
					["Customer Helper Name"] = "Assistant client {0}",
					["Security Guard Name"] = "Agent de sécurité {0}",
					["Janitor Name"] = "Concierge {0}",
					["Lvl"] = "<size=70%>Niv </size>{0}",
					["Popup Exp"] = "{0} Pex",
					["Level"] = "Niveau {0} - {1}",
					["TRAINING"] = "FORMATION",
					["Cashiers"] = "Caissiers",
					["Restockers"] = "Restockers",
					["Customer Helpers"] = "<size=80%>Assistant\nclient</size>",
					["Security Guards"] = "<size=80%>Agents de\nsécurité</size>",
					["Janitors"] = "Concierges",
					["Exp."] = "Pex",
					["Scans Per Minute"] = "Scans par minute",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Temps de paiement",
					["Payment Time Sec."] = "{0} sec.",
					["Daily Wage"] = "Salaire journalier",
					["Rapidity"] = "Rapidité",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacité",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Dextérité",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Feuille de route de la maîtrise",
					["Rookie"] = "Débutant",
					["Middle"] = "Moyen",
					["Advance"] = "Progression",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Verrouillé",
					["Approve"] = "Approuver",
					["Defer"] = "Reporter",
					["Train to Level Up"] = "Passer au niv. suivant",
					["Unlock Higher Grade"] = "Débloquer le prochain",
					["Show head gauge"] = "Montrer la jauge au-dessus de la tête",
					["Upgrade warning"] = "Seriez-vous prêt à promouvoir le {0}\nau grade {1} ?\n\nIl sera encore plus efficace mais\nson salaire journalier passera à {2}</size>.",
					["NO CASHIERS HIRED"] = "PAS D'CAISSIERS EMBAUCHÉS",
					["NO RESTOCKERS HIRED"] = "PAS D'RESTOCKERS EMBAUCHÉS",
					["NO CUSTOMER HELPERS HIRED"] = "PAS D'ASSISTANT CLIENT EMBAUCHÉS",
					["NO SECURITY GUARDS HIRED"] = "PAS D'AGENTS DE SÉCURITÉ EMBAUCHÉS",
					["NO JANITORS HIRED"] = "PAS D'CONCIERGES EMBAUCHÉS",
				},
				new Localizations("pt-PT") { // ポルトガル
					["Cashier Name"] = "Caixa {0}",
					["Restocker Name"] = "Repositor {0}",
					["Customer Helper Name"] = "A judante do cliente {0}",
					["Security Guard Name"] = "Gauarda de segurança {0}",
					["Janitor Name"] = "Zelador {0}",
					["Lvl"] = "<size=70%>Nív </size>{0}",
					["Popup Exp"] = "{0} Exp",
					["Level"] = "Nível {0} - {1}",
					["TRAINING"] = "TREINAMENTO",
					["Cashiers"] = "Caixas",
					["Restockers"] = "Repositores",
					["Customer Helpers"] = "<size=80%>A judante do\ncliente</size>",
					["Security Guards"] = "<size=80%>Guardas de\nsegurança</size>",
					["Janitors"] = "Zeladores",
					["Exp."] = "Exp.",
					["Scans Per Minute"] = "Leituras por minuto",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Tempo de pagamento",
					["Payment Time Sec."] = "{0} seg.",
					["Daily Wage"] = "Salário diário",
					["Rapidity"] = "Rapidez",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacidade",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Destreza",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Roteiro de domínio",
					["Rookie"] = "Novato",
					["Middle"] = "Médio",
					["Advance"] = "Avançado",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "bloqueado",
					["Approve"] = "Aprovar",
					["Defer"] = "Adiar",
					["Train to Level Up"] = "Treinar para subir de nível",
					["Unlock Higher Grade"] = "Desbloquear grau superior",
					["Show head gauge"] = "Mostrar o medidor em cima",
					["Upgrade warning"] = "Promoveria o {0} ao grau {1}?\n\nEle será ainda mais eficiente mas\no seu salário diário AUMENTARÁ para {2}</size>.",
					["NO CASHIERS HIRED"] = "NÃO FORAM CONTRATADOS CAIXAS",
					["NO RESTOCKERS HIRED"] = "NÃO FORAM CONTRATADOS REPOSITORES",
					["NO CUSTOMER HELPERS HIRED"] = "NÃO FORAM CONTRATADOS AJUDANTE DO CLIENTE",
					["NO SECURITY GUARDS HIRED"] = "NÃO FORAM CONTRATADOS GUARDAS DE SEGURANÇA",
					["NO JANITORS HIRED"] = "NÃO FORAM CONTRATADOS ZELADORES",
				},
				new Localizations("pt-BR") { // ブラジル・ポルトガル
					["Cashier Name"] = "Caixa {0}",
					["Restocker Name"] = "Repositor {0}",
					["Customer Helper Name"] = "A judante do cliente {0}",
					["Security Guard Name"] = "Gauarda de segurança {0}",
					["Janitor Name"] = "Zelador {0}",
					["Lvl"] = "<size=70%>Nív </size>{0}",
					["Popup Exp"] = "{0} Exp",
					["Level"] = "Nível {0} - {1}",
					["TRAINING"] = "TREINAMENTO",
					["Cashiers"] = "Caixas",
					["Restockers"] = "Repositores",
					["Customer Helpers"] = "<size=80%>Ajudante do\ncliente</size>",
					["Security Guards"] = "<size=80%>Guardas de\nsegurança</size>",
					["Janitors"] = "Zeladores",
					["Exp."] = "Exp.",
					["Scans Per Minute"] = "Escaneamentos por minuto",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Tempo de pagamento",
					["Payment Time Sec."] = "{0} seg.",
					["Daily Wage"] = "Salário diário",
					["Rapidity"] = "Rapidez",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacidade",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Destreza",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Roteiro de domínio",
					["Rookie"] = "Novato",
					["Middle"] = "Médio",
					["Advance"] = "Avançado",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Bloqueado",
					["Approve"] = "Aprovar",
					["Defer"] = "Adiar",
					["Train to Level Up"] = "Treinar para subir de nível",
					["Unlock Higher Grade"] = "Desbloquear grau superior",
					["Show head gauge"] = "Mostrar o medidor na cabeça",
					["Upgrade warning"] = "Promoveria o {0} ao grau {1}?\n\nEle será ainda mais eficiente mas\no seu salário diário AUMENTARÁ para {2}</size>.",
					["NO CASHIERS HIRED"] = "NENHUM CAIXA CONTRATADO",
					["NO RESTOCKERS HIRED"] = "NENHUM REPOSITOR CONTRATADO",
					["NO CUSTOMER HELPERS HIRED"] = "NENHUM AJUDANTE DO CLIENTE CONTRATADO",
					["NO SECURITY GUARDS HIRED"] = "NENHUM GUARDAS DE SEGURANÇA CONTRATADO",
					["NO JANITORS HIRED"] = "NENHUM ZELADORES CONTRATADO",
				},
				new Localizations("de") { // ドイツ
					["Cashier Name"] = "Kassierer {0}",
					["Restocker Name"] = "Auffüller {0}",
					["Customer Helper Name"] = "Kundenhelfer {0}",
					["Security Guard Name"] = "Sicherheitsbeamter {0}",
					["Janitor Name"] = "Hausmeister {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} EP",
					["Level"] = "Stufe {0} - {1}",
					["TRAINING"] = "AUSBILDUNG",
					["Cashiers"] = "Kassierer",
					["Restockers"] = "Auffüller",
					["Customer Helpers"] = "<size=85%>Kundenhelfer</size>",
					["Security Guards"] = "<size=80%>Sicherheits-\nleute</size>",
					["Janitors"] = "Hausmeister",
					["Exp."] = "EP",
					["Scans Per Minute"] = "Scans pro Minute",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Zahlungszeit",
					["Payment Time Sec."] = "{0} Sek.",
					["Daily Wage"] = "Täglicher Lohn",
					["Rapidity"] = "Zügigkeit",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Tragfähigkeit",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Fingerfertigkeit",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Mastery-Fahrplan",
					["Rookie"] = "Neueinsteiger",
					["Middle"] = "Mittel",
					["Advance"] = "Fortgeschritten",
					["Pro"] = "Profi",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Gesperrt",
					["Approve"] = "Genehmigen",
					["Defer"] = "Aufschieben",
					["Train to Level Up"] = "Trainieren, um aufzusteigen",
					["Unlock Higher Grade"] = "Obere Stufe freischalten",
					["Show head gauge"] = "Zeigen Sie das Messgerät über dem Kopf",
					["Upgrade warning"] = "Würden Sie Kassierer {0}\nzum {1} grad befördern?\n\nEr wird noch effizienter sein aber\nsein Tageslohn wird auf {2}</size> ERHÖHT.",
					["NO CASHIERS HIRED"] = "KEINE KASSIERER EINGESTELLT",
					["NO RESTOCKERS HIRED"] = "KEINE AUFFÜLLER EINGESTELLT",
					["NO CUSTOMER HELPERS HIRED"] = "KEINE KUNDENHELFER EINGESTELLT",
					["NO SECURITY GUARDS HIRED"] = "KEINE SICHERHEITSLEUTE EINGESTELLT",
					["NO JANITORS HIRED"] = "KEINE HAUSMEISTER EINGESTELLT",
				},
				new Localizations("da") { // デンマーク
					["Cashier Name"] = "Kasserer {0}",
					["Restocker Name"] = "Opfylder {0}",
					["Customer Helper Name"] = "Kundehjælper {0}",
					["Security Guard Name"] = "Sikkerhedsvagt {0}",
					["Janitor Name"] = "Vicevært {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} EP",
					["Level"] = "Niveau {0} - {1}",
					["TRAINING"] = "UDDANNELSE",
					["Cashiers"] = "Kasserere",
					["Restockers"] = "Opfyldere",
					["Customer Helpers"] = "<size=80%>Kundehjælpere</size>",
					["Security Guards"] = "<size=80%>Sikkerhedsvagter</size>",
					["Janitors"] = "Viceværter",
					["Exp."] = "EP",
					["Scans Per Minute"] = "Scanninger pr. minut",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Tid til betaling",
					["Payment Time Sec."] = "{0} sek.",
					["Daily Wage"] = "Daglig løn",
					["Rapidity"] = "Fodhastighed",
					["Speed"] = "{0}<size=70%> km/t</size>",
					["Capacity"] = "Kapacitet",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Fingerfærdighed",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Køreplan for mestring",
					["Rookie"] = "Rookie",
					["Middle"] = "Mellem",
					["Advance"] = "Fremskreden",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Låst",
					["Approve"] = "Godkend",
					["Defer"] = "Udsæt",
					["Train to Level Up"] = "Træn for at stige i niveau",
					["Unlock Higher Grade"] = "Lås op for øvre niveau",
					["Show head gauge"] = "Vis måleren over hovedet",
					["Upgrade warning"] = "Vil du forfremme {0} til {1} klassen?\n\nDen vil være endnu mere effektiv men\ndens daglige løn vil STIGE til {2}</size>.",
					["NO CASHIERS HIRED"] = "INGEN KASSEDAMER ANSAT",
					["NO RESTOCKERS HIRED"] = "INGEN OPFYLDERE ANSAT",
					["NO CUSTOMER HELPERS HIRED"] = "INGEN KUNDEHJÆLPERE ANSAT",
					["NO SECURITY GUARDS HIRED"] = "INGEN SIKKERHEDSVAGTER ANSAT",
					["NO JANITORS HIRED"] = "INGEN VICEVÆRTER ANSAT",
				},
				new Localizations("pl") { // ポーランド
					["Cashier Name"] = "Kasjera {0}",
					["Restocker Name"] = "Restockera {0}",
					["Customer Helper Name"] = "Pomocnik klienta {0}",
					["Security Guard Name"] = "Ochrona {0}",
					["Janitor Name"] = "Dozorca {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} PD",
					["Level"] = "Poziom {0} - {1}",
					["TRAINING"] = "SZKOLENIE",
					["Cashiers"] = "Kasjerzy",
					["Restockers"] = "Restockerzy",
					["Customer Helpers"] = "<size=80%>Pomocnicy\nklienta</size>",
					["Security Guards"] = "Ochroniarze",
					["Janitors"] = "Dozorcy",
					["Exp."] = "PD",
					["Scans Per Minute"] = "Skany na minutę",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Czas płatności",
					["Payment Time Sec."] = "{0} sek.",
					["Daily Wage"] = "Dzienne wynagrodzenie",
					["Rapidity"] = "Prędkość",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Ładowność",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Zręczność",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Mapa drogowa mistrzostwa",
					["Rookie"] = "Żółtodziób",
					["Middle"] = "Środek",
					["Advance"] = "Zaawansowany",
					["Pro"] = "Zawodowiec",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Zablokowany",
					["Approve"] = "Zatwierdzić",
					["Defer"] = "Odroczenie",
					["Train to Level Up"] = "Trenuj, aby awansować",
					["Unlock Higher Grade"] = "Odblokowanie wyższego poziomu",
					["Show head gauge"] = "Pokaż skrajnię nad głową",
					["Upgrade warning"] = "Czy awansowałbyś Kasjera {0} na {1}?\n\nBędzie on jeszcze bardziej wydajny ale\njego dzienne wynagrodzenie wzrośnie do {2}</size>.",
					["NO CASHIERS HIRED"] = "NIE ZATRUDNIONO KASJERÓW",
					["NO RESTOCKERS HIRED"] = "NIE ZATRUDNIONO MAGAZYNIERÓW",
					["NO CUSTOMER HELPERS HIRED"] = "NIE ZATRUDNIONO POMOCNIKÓW KLIENTA",
					["NO SECURITY GUARDS HIRED"] = "NIE ZATRUDNIONO OCHRONIARZE",
					["NO JANITORS HIRED"] = "NIE ZATRUDNIONO DOZORCY",
				},
				new Localizations("nl-NL") { // オランダ
					["Cashier Name"] = "Kassier {0}",
					["Restocker Name"] = "Vakkenvuller {0}",
					["Customer Helper Name"] = "Klantenhelper {0}",
					["Security Guard Name"] = "Bewaker {0}",
					["Janitor Name"] = "Conciërge {0}",
					["Lvl"] = "<size=70%>Niv </size>{0}",
					["Popup Exp"] = "{0} EP",
					["Level"] = "Niveau {0} - {1}",
					["TRAINING"] = "OPLEIDING",
					["Cashiers"] = "Kassiers",
					["Restockers"] = "<size=80%>Vakkenvullers</size>",
					["Customer Helpers"] = "<size=80%>Klantenhelpers</size>",
					["Security Guards"] = "Beveiligers",
					["Janitors"] = "Conciërges",
					["Exp."] = "EP",
					["Scans Per Minute"] = "Scans per minuut",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Betalingstijd",
					["Payment Time Sec."] = "{0} sec.",
					["Daily Wage"] = "Dagloon",
					["Rapidity"] = "Vlugheid",
					["Speed"] = "{0}<size=70%> km/u</size>",
					["Capacity"] = "Laadcapaciteit",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Handigheid",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Meesterschap stappenplan",
					["Rookie"] = "Beginner",
					["Middle"] = "Midden",
					["Advance"] = "Gevorderd",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Gesloten",
					["Approve"] = "Goedkeuren",
					["Defer"] = "Uitstellen",
					["Train to Level Up"] = "Train om omhoog te komen",
					["Unlock Higher Grade"] = "Hogere rang vrijspelen",
					["Show head gauge"] = "Toon het bovenleidingsprofiel",
					["Upgrade warning"] = "Zou je kassier {0} promoveren\nnaar de {1}-rang?\n\nHij zal nog efficiënter zijn maar\nzijn dagloon zal VERHOGEN naar {2}</size>.",
					["NO CASHIERS HIRED"] = "GEEN KASSIERS AANGENOMEN",
					["NO RESTOCKERS HIRED"] = "GEEN VAKKENVULLERS AANGENOMEN",
					["NO CUSTOMER HELPERS HIRED"] = "GEEN KLANTENHELPERS AANGENOMEN",
					["NO SECURITY GUARDS HIRED"] = "GEEN BEVEILIGERS AANGENOMEN",
					["NO JANITORS HIRED"] = "GEEN CONCIËRGES AANGENOMEN",
				},
				new Localizations("es-ES") { // スペイン
					["Cashier Name"] = "Cajero/a {0}",
					["Restocker Name"] = "Reponedor {0}",
					["Customer Helper Name"] = "Ayudante del cliente {0}",
					["Security Guard Name"] = "Guardia de seguridad {0}",
					["Janitor Name"] = "Conserje {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} Exp",
					["Level"] = "Livello {0} - {1}",
					["TRAINING"] = "FORMACIÓN",
					["Cashiers"] = "Cajeros/as",
					["Restockers"] = "Reponedores",
					["Customer Helpers"] = "<size=80%>Ayudantes del\ncliente</size>",
					["Security Guards"] = "<size=80%>Guardias de\nseguridad</size>",
					["Janitors"] = "Conserjes",
					["Exp."] = "Exp.",
					["Scans Per Minute"] = "Escaneos por minuto",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Tiempo de pago",
					["Payment Time Sec."] = "{0} seg.",
					["Daily Wage"] = "Salario diario",
					["Rapidity"] = "Rapidez",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "Capacidad de carga",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "Destreza",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Hoja de ruta de maestría",
					["Rookie"] = "Novato",
					["Middle"] = "Medio",
					["Advance"] = "Avanzar",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Bloqueado",
					["Approve"] = "Aprobar",
					["Defer"] = "Aplazar",
					["Train to Level Up"] = "Entrenar para subir de nivel",
					["Unlock Higher Grade"] = "Desbloquear grado superior",
					["Show head gauge"] = "Mostrar el gálibo superior",
					["Upgrade warning"] = "¿Ascenderías a {0} al grado {1}?\n\n Será aún más eficiente pero\nsu salario diario AUMENTARÁ a {2}</size>.",
					["NO CASHIERS HIRED"] = "NO HAY CAJEROS CONTRATADOS",
					["NO RESTOCKERS HIRED"] = "NO HAY REPONEDORES CONTRATADOS",
					["NO CUSTOMER HELPERS HIRED"] = "NO HAY AYUDANTES DEL CLIENTE CONTRATADOS",
					["NO SECURITY GUARDS HIRED"] = "NO HAY GUARDIAS DE SEGURIDAD CONTRATADOS",
					["NO JANITORS HIRED"] = "NO HAY CONSERJES CONTRATADOS",
				},
				new Localizations("tr") { // トルコ
					["Cashier Name"] = "Kasiyer {0}",
					["Restocker Name"] = "Depo Görevlisi {0}",
					["Customer Helper Name"] = "Müsteri Yardımcısı {0}",
					["Security Guard Name"] = "Güvenlik Görevlisi {0}",
					["Janitor Name"] = "Temizlikci {0}",
					["Lvl"] = "<size=70%>Lvl </size>{0}",
					["Popup Exp"] = "{0} EXP",
					["Level"] = "{0}. Seviye - {1}",
					["TRAINING"] = "EĞİTİM",
					["Cashiers"] = "Kasiyerler",
					["Restockers"] = "<size=80%>Depo\nGörevlileri</size>",
					["Customer Helpers"] = "<size=80%>Müsteri\nYardımcıları</size>",
					["Security Guards"] = "<size=80%>Güvenlik\nGörevlileri</size>",
					["Janitors"] = "<size=95%>Temizlikciler</size>",
					["Exp."] = "EXP",
					["Scans Per Minute"] = "Dakika başına tarama",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Ödeme süresi",
					["Payment Time Sec."] = "{0} saniye.",
					["Daily Wage"] = "Günlük ücret",
					["Rapidity"] = "Hızlılık",
					["Speed"] = "{0}<size=70%> km/s</size>",
					["Capacity"] = "Kapasite",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "El becerisi",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Ustalık yol haritası",
					["Rookie"] = "Çaylak",
					["Middle"] = "Orta",
					["Advance"] = "İlerleme",
					["Pro"] = "Pro",
					["Ninja"] = "Ninja",
					["?"] = "?",
					["Locked"] = "Kilitli",
					["Approve"] = "Onaylamak",
					["Defer"] = "Erteleme",
					["Train to Level Up"] = "Seviye atlamak için eğitin",
					["Unlock Higher Grade"] = "Üst sınıfın kilidini aç",
					["Show head gauge"] = "Göstergeyi başınızın üzerinde gösterin",
					["Upgrade warning"] = "{0}'i {1} sınıfına terfi ettirir misiniz?\n\nDaha da verimli olacaktır ama\ngünlük ücreti {2}</size> yükselecektir.",
					["NO CASHIERS HIRED"] = "KASIYER IŞE ALINMADI",
					["NO RESTOCKERS HIRED"] = "DEPO GÖREVLİ IŞE ALINMADI",
					["NO CUSTOMER HELPERS HIRED"] = "MÜSTERİ YARDIMCILARI IŞE ALINMADI",
					["NO SECURITY GUARDS HIRED"] = "GÜVENLİK GÖREVLİLERİ IŞE ALINMADI",
					["NO JANITORS HIRED"] = "TEMİZLİKÇİLER IŞE ALINMADI",
				},
				new Localizations("zh-CN") { // 簡体
					["Cashier Name"] = "收银员 {0}",
					["Restocker Name"] = "补货员 {0}",
					["Customer Helper Name"] = "客户助理 {0}",
					["Security Guard Name"] = "警卫 {0}",
					["Janitor Name"] = "清洁工 {0}",
					["Lvl"] = "{0}<size=70%>级</size>",
					["Popup Exp"] = "{0} EXP",
					["Level"] = "{0} 级 - {1}",
					["TRAINING"] = "训练",
					["Cashiers"] = "收银员",
					["Restockers"] = "补货员",
					["Customer Helpers"] = "客户助理",
					["Security Guards"] = "警卫",
					["Janitors"] = "清洁工",
					["Exp."] = "EXP",
					["Scans Per Minute"] = "每分钟扫描次数",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "付款时间",
					["Payment Time Sec."] = "{0} 秒",
					["Daily Wage"] = "每日工资",
					["Rapidity"] = "敏捷性",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "装载能力",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "灵巧性",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "精通路线图",
					["Rookie"] = "新手",
					["Middle"] = "中级",
					["Advance"] = "高级",
					["Pro"] = "专业职",
					["Ninja"] = "忍者",
					["?"] = "?",
					["Locked"] = "锁定",
					["Approve"] = "批准",
					["Defer"] = "推迟",
					["Train to Level Up"] = "训练直到你升级",
					["Unlock Higher Grade"] = "解锁更高等级",
					["Show head gauge"] = "在头部上方显示仪表",
					["Upgrade warning"] = "您会将 {0} 提升为“{1}”别吗？\n\n他的效率会更高，但日薪也会增加到 {2}</size>。",
					["NO CASHIERS HIRED"] = "没有雇用收银员",
					["NO RESTOCKERS HIRED"] = "没有雇用补货员",
					["NO CUSTOMER HELPERS HIRED"] = "没有雇用客户助理",
					["NO SECURITY GUARDS HIRED"] = "没有雇用警卫",
					["NO JANITORS HIRED"] = "没有雇用清洁工",
				},
				new Localizations("zh-TW") { // 繁体
					["Cashier Name"] = "收銀員 {0}",
					["Restocker Name"] = "補貨員 {0}",
					["Customer Helper Name"] = "客戶幫手 {0}",
					["Security Guard Name"] = "保安員 {0}",
					["Janitor Name"] = "清潔員 {0}",
					["Lvl"] = "{0}<size=70%>級</size>",
					["Popup Exp"] = "{0} EXP",
					["Level"] = "{0} 級 - {1}",
					["TRAINING"] = "訓練",
					["Cashiers"] = "收銀員",
					["Restockers"] = "補貨員",
					["Customer Helpers"] = "客戶幫手",
					["Security Guards"] = "保安員",
					["Janitors"] = "清潔員",
					["Exp."] = "EXP",
					["Scans Per Minute"] = "每分鐘掃描次數",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "付款時間",
					["Payment Time Sec."] = "{0} 秒",
					["Daily Wage"] = "每日工資",
					["Rapidity"] = "敏捷性",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "裝載能力",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "靈巧",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "精通路線圖",
					["Rookie"] = "新人",
					["Middle"] = "中級",
					["Advance"] = "高級",
					["Pro"] = "專業級",
					["Ninja"] = "忍者",
					["?"] = "？",
					["Locked"] = "鎖定",
					["Approve"] = "核准",
					["Defer"] = "推遲",
					["Train to Level Up"] = "訓練直到你升級",
					["Unlock Higher Grade"] = "解鎖更高等級",
					["Show head gauge"] = "在頭部上方顯示儀表",
					["Upgrade warning"] = "您會將 {0} 提升為“{1}”嗎？\n\n效率會更高，但日薪將增加至 {2}</size>。",
					["NO CASHIERS HIRED"] = "沒有僱用收銀員",
					["NO RESTOCKERS HIRED"] = "沒有僱用補貨員",
					["NO CUSTOMER HELPERS HIRED"] = "沒有僱用客戶幫手",
					["NO SECURITY GUARDS HIRED"] = "沒有僱用保安員",
					["NO JANITORS HIRED"] = "沒有僱用清潔員",
				},
				new Localizations("ko-KR") { // 韓国
					["Cashier Name"] = "계산원 {0}",
					["Restocker Name"] = "재입고 {0}",
					["Customer Helper Name"] = "고객 도우미 {0}",
					["Security Guard Name"] = "경비원 {0}",
					["Janitor Name"] = "청소부 {0}",
					["Lvl"] = "<size=70%>레벨 </size>{0}",
					["Popup Exp"] = "{0} EXP",
					["Level"] = "레벨 {0} - {1}",
					["TRAINING"] = "훈련",
					["Cashiers"] = "계산원",
					["Restockers"] = "재입고",
					["Customer Helpers"] = "고객 도우미",
					["Security Guards"] = "경비원",
					["Janitors"] = "청소부",
					["Exp."] = "EXP",
					["Scans Per Minute"] = "분당 스캔 수",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "결제 시간",
					["Payment Time Sec."] = "{0}초",
					["Daily Wage"] = "일일 임금",
					["Rapidity"] = "신속성",
					["Speed"] = "{0}<size=70%> km/h</size>",
					["Capacity"] = "적재량",
					["Weight/Height"] = "{0}<size=70%> kg</size> / {1}<size=70%> m</size>",
					["Dexterity"] = "민첩성",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "마스터리 로드맵",
					["Rookie"] = "루키",
					["Middle"] = "중급",
					["Advance"] = "고급",
					["Pro"] = "프로",
					["Ninja"] = "닌자",
					["?"] = "?",
					["Locked"] = "잠김",
					["Approve"] = "승인",
					["Defer"] = "보류",
					["Train to Level Up"] = "레벨 업 훈련",
					["Unlock Higher Grade"] = "상위 등급 잠금 해제",
					["Show head gauge"] = "머리 위로 게이지 표시",
					["Upgrade warning"] = "{0} 을 『{1} 등급으』로 승격하시겠습니까?\n\n훨씬 더 효율적이겠지만\n일일 임금은 {2}</size>로 인상됩니다.",
					["NO CASHIERS HIRED"] = "고용된 계산원 없음",
					["NO RESTOCKERS HIRED"] = "고용된 재입고 없음",
					["NO CUSTOMER HELPERS HIRED"] = "고용된 고객 도우미 없음",
					["NO SECURITY GUARDS HIRED"] = "고용된 경비원 없음",
					["NO JANITORS HIRED"] = "고용된 청소부 없음",
				},
				new Localizations("ru-RU") { // ロシア
					["Cashier Name"] = "Кассир {0}",
					["Restocker Name"] = "Кладовщик {0}",
					["Customer Helper Name"] = "<size=50%>Помощник по работе с клиентами </size>{0}",
					["Security Guard Name"] = "Охранник {0}",
					["Janitor Name"] = "Уборщик {0}",
					["Lvl"] = "<size=70%>Ур </size>{0}",
					["Popup Exp"] = "{0} Экспа",
					["Level"] = "Уровень {0} - {1}",
					["TRAINING"] = "ОБУЧЕНИЕ",
					["Cashiers"] = "Кассиры",
					["Restockers"] = "<size=80%>Кладовщики</size>",
					["Customer Helpers"] = "<size=80%>Помощники\nклиентов</size>",
					["Security Guards"] = "Охранники",
					["Janitors"] = "Уборщики",
					["Exp."] = "Экспа",
					["Scans Per Minute"] = "Сканирование в минуту",
					["SPM Range"] = "{0} - {1}",
					["Payment Time"] = "Время оплаты",
					["Payment Time Sec."] = "{0} сек.",
					["Daily Wage"] = "Ежедневная зарплата",
					["Rapidity"] = "Быстрота",
					["Speed"] = "{0}<size=70%> км/ч</size>",
					["Capacity"] = "Грузоподъемность",
					["Weight/Height"] = "{0}<size=70%> кг</size> / {1}<size=70%> м</size>",
					["Dexterity"] = "Ловкость",
					["Percentage"] = "{0}<size=70%>%</size>",
					["Mastery Roadmap"] = "Дорожная карта мастерства",
					["Rookie"] = "Новичок",
					["Middle"] = "Средний",
					["Advance"] = "Продвинутый",
					["Pro"] = "Профи",
					["Ninja"] = "Ниндзя",
					["?"] = "?",
					["Locked"] = "Заперто",
					["Approve"] = "Одобрить",
					["Defer"] = "Отложить",
					["Train to Level Up"] = "Уровень повышен",
					["Unlock Higher Grade"] = "Улучшать",
					["Show head gauge"] = "Показать датчик на голове",
					["Upgrade warning"] = "Вы бы повысили класс кассира {0}\nдо {1}?\nОн будет работать еще эффективнее но\nего дневная зарплата увеличится до {2}</size>.",
					["NO CASHIERS HIRED"] = "КАССИРЫ НЕ НАНЯТЫ",
					["NO RESTOCKERS HIRED"] = "КЛАДОВЩИКИ НЕ НАНЯТЫ",
					["NO CUSTOMER HELPERS HIRED"] = "ПОМОЩНИКИ КЛИЕНТОВ НЕ НАНЯТЫ",
					["NO SECURITY GUARDS HIRED"] = "ОХРАННИКИ НЕ НАНЯТЫ",
					["NO JANITORS HIRED"] = "УБОРЩИКИ НЕ НАНЯТЫ",
				},
			};

			if (settings.CustomizeLocalization) {
                Write(langs);
			}
			return langs;
		}
	}

}