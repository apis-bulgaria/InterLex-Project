/*--- START OF EXAMPLE CONSTANTS ---*/
var EXAMPLE_CASELAW_PARTIES = "Smith",
    EXAMPLE_CASELAW_CELEX = "62014CJ0032",
    EXAMPLE_CASELAW_DATE = "08/10/2007", //Date_From
    EXAMPLE_CASELAW_COURT_FOLDERS = { //Germany / Bundesgerichtshof 
        KeyPaths: "/23788d1d-b892-4479-8e80-f34a97bb9409/27397853-bec8-4975-bc4d-e1c834268056",
        SelectedIds: "27397853-bec8-4975-bc4d-e1c834268056"
    },
    EXAMPLE_CASELAW_COURT_FOLDERS_BG = { //България
        KeyPaths: "/c28edf6b-4597-4056-92af-3f34a2a5b1a0",
        SelectedIds: "c28edf6b-4597-4056-92af-3f34a2a5b1a0"
    },
    EXAMPLE_CASELAW_EUROVOC = { //FINANCE / taxation
        KeyPaths: "/578975f1-3053-487e-8a92-222c876fcdc5/5fb24906-78a0-4a8e-98d1-26cee3702a85",
        SelectedIds: "5fb24906-78a0-4a8e-98d1-26cee3702a85"
    },
    EXAMPLE_CASELAW_EUROVOC_BG = { //РАБОТА И УСЛОВИЯ НА ТРУД
        KeyPaths: "/df782a10-ac6e-49eb-98f8-74f88a8815bd",
        SelectedIds: "df782a10-ac6e-49eb-98f8-74f88a8815bd"
    },
    EXAMPLE_CASELAW_SUBJECT_MATTER = { //anti-discrimination (DISC)
        KeyPaths: "/865cc1f3-7a91-4450-9a54-1300e64c6736",
        SelectedIds: "865cc1f3-7a91-4450-9a54-1300e64c6736"
    },
    EXAMPLE_CASELAW_DIRECTORY = { //Classification scheme after the Lisbon Treaty (2010) / Internal policy of the European Union / State aid
        KeyPaths: "/32016c73-e519-447a-a222-a7ccfe2ae675/133e04a7-1074-4bda-914e-305fe142fd89/5ab9393a-b894-4bc6-897f-42cbdd0e372b",
        SelectedIds: "5ab9393a-b894-4bc6-897f-42cbdd0e372b"
    },
    EXAMPLE_CASELAW_DIRECTORY_BG = { //Kласифициране след Договора от Лисабон (2010) / Съдебни спорове 
        KeyPaths: "/32016c73-e519-447a-a222-a7ccfe2ae675/13eece58-8f2e-46f0-a85a-bf0818c76653",
        SelectedIds: "13eece58-8f2e-46f0-a85a-bf0818c76653"
    },
    EXAMPLE_CASELAW_EUROCASES = { //Labour Law / Safety and health at work
        KeyPaths: "/ec80ea93-7aab-744d-971a-b4e9d7cabd73/dd9991eb-0a01-3a48-b6c0-cd4c4c481ccd",
        SelectedIds: "dd9991eb-0a01-3a48-b6c0-cd4c4c481ccd"
    },
    EXAMPLE_CASELAW_DOCUMENT_TYPE = { //Case law / EU case law / Third-party proceedings 
        KeyPaths: "/1a5f1eae-99a5-452e-a919-f2f3d11d3168/030bc6ca-9696-4efe-ba4b-27d54a335726/6e47a456-9fd2-4f5b-bd6d-71634a4c2bd9",
        SelectedIds: "6e47a456-9fd2-4f5b-bd6d-71634a4c2bd9"
    },
    EXAMPLE_CASELAW_PROCEDURE_TYPE = { //Action for damages
        KeyPaths: "/fc2683cf-e6c7-4a25-a439-417180debb35",
        SelectedIds: "fc2683cf-e6c7-4a25-a439-417180debb35"
    },
    EXAMPLE_CASELAW_ADVOCATE_GENERAL = { //Fennelly
        KeyPaths: "/dc1bb33a-0c50-47be-9228-1446c6e006b1",
        SelectedIds: "dc1bb33a-0c50-47be-9228-1446c6e006b1"
    },
    EXAMPLE_CASELAW_JUDGE_RAPPORTEUR = { //Berger
        KeyPaths: "/cf72f49c-713b-4a1f-8bec-e2b48739321e",
        SelectedIds: "cf72f49c-713b-4a1f-8bec-e2b48739321e"
    },
    EXAMPLE_CASELAW_LEGAL_ACT = {
        Text: '31977L0489: Council Directive 77/489/EEC of 18 July 1977 on the protection of animals during international transport',
        Id: '3853857'
    },
    EXAMPLE_CASELAW_PROVISION = {
        Text: 'Article 2',
        TextBG: 'Член 2',
        Id: '779087'
    },
    EXAMPLE_LEGISLATION_DIRECTORY = { //Competition policy 
        KeyPaths: "/7178e1ff-d051-44d4-ad77-f82e4f9d54a5",
        SelectedIds: "7178e1ff-d051-44d4-ad77-f82e4f9d54a5"
    },
    EXAMPLE_LEGISLATION_CELEX = "32015R1733",
    EXAMPLE_LEGISLATION_DATE = "08/10/2007",
    EXAMPLE_LEGISLATION_EUROVOC = { //EMPLOYMENT AND WORKING CONDITIONS 
        KeyPaths: "/df782a10-ac6e-49eb-98f8-74f88a8815bd",
        SelectedIds: "df782a10-ac6e-49eb-98f8-74f88a8815bd"
    },
    EXAMPLE_LEGISLATION_SUBJECT_MATTER = { //anti-discrimination (DISC) 
        KeyPaths: "/865cc1f3-7a91-4450-9a54-1300e64c6736",
        SelectedIds: "865cc1f3-7a91-4450-9a54-1300e64c6736"
    },
    EXAMPLE_LEGISLATION_DOCUMENT_TYPE = { //EU legislation / Treaties 
        KeyPaths: "/af88ca51-7522-455a-aefe-ec0d3c2d6a37/f83a4979-0348-43e0-a7b6-d50c07d4eee9",
        SelectedIds: "f83a4979-0348-43e0-a7b6-d50c07d4eee9"
    },
    EXAMPLE_LOGICAL_EXPRESSIONS = "tax [AND] credit [NOT] corporate",
    EXAMPLE_LOGICAL_EXPRESSIONS_BG = "данъчен [AND] кредит [NOT] корпоративен"
/*--- END OF EXAMPLE CONSTANTS ---*/

function hintExampleCaseLawParties(lang) {
    clearSearchBox('CaseLaw');
    $("#Cases_Parties").val(EXAMPLE_CASELAW_PARTIES);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawCelex(lang) {
    clearSearchBox('CaseLaw');
    $('#Cases_NatID_ECLI').val(EXAMPLE_CASELAW_CELEX);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawJurisdiction(lang) {
    clearSearchBox('CaseLaw');
    if (lang === 'bg') {
        populateTree('Cases_CourtsFolders', EXAMPLE_CASELAW_COURT_FOLDERS_BG);
    }
    else {
        populateTree('Cases_CourtsFolders', EXAMPLE_CASELAW_COURT_FOLDERS);
    }

    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawDate() {
    $('#Cases_DateFrom').val(EXAMPLE_CASELAW_DATE);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawLegalAct(lang) {
    clearSearchBox('CaseLaw');
    $('#Cases_EnactmentText').val(EXAMPLE_CASELAW_LEGAL_ACT.Text);
    $('#Cases_EnactmentText_Index').val(EXAMPLE_CASELAW_LEGAL_ACT.Text);
    $('#Cases_EnactmentDocLangId').val(EXAMPLE_CASELAW_LEGAL_ACT.Id).trigger('change');
    $('#Cases_EnactmentDocLangId_Index').val(EXAMPLE_CASELAW_LEGAL_ACT.Id).trigger('change');
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawProvision(lang) {
    clearSearchBox('CaseLaw');
    $('#Cases_EnactmentText').val(EXAMPLE_CASELAW_LEGAL_ACT.Text);
    $('#Cases_EnactmentText_Index').val(EXAMPLE_CASELAW_LEGAL_ACT.Text);
    $('#Cases_EnactmentDocLangId').val(EXAMPLE_CASELAW_LEGAL_ACT.Id).trigger('change');
    $('#Cases_EnactmentDocLangId_Index').val(EXAMPLE_CASELAW_LEGAL_ACT.Id).trigger('change');
    if (lang === 'bg') {
        $('#Cases_ProvisionText').val(EXAMPLE_CASELAW_PROVISION.TextBG);
        $('#Cases_ProvisionText_Index').val(EXAMPLE_CASELAW_PROVISION.TextBG);
    }
    else {
        $('#Cases_ProvisionText').val(EXAMPLE_CASELAW_PROVISION.Text);
        $('#Cases_ProvisionText_Index').val(EXAMPLE_CASELAW_PROVISION.Text);
    }

    $('#Cases_ProvisionId').val(EXAMPLE_CASELAW_PROVISION.Id).trigger('change');
    $('#Cases_ProvisionId_Index').val(EXAMPLE_CASELAW_PROVISION.Id).trigger('change');
    DoSearch('tabCases', false);
    return false;
}

function hintExampleDocumentType(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_DocumentTypes', EXAMPLE_CASELAW_DOCUMENT_TYPE);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleProcedureType(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_ProcedureType', EXAMPLE_CASELAW_PROCEDURE_TYPE);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawAdvocateGeneral(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_AdvocateGeneral', EXAMPLE_CASELAW_ADVOCATE_GENERAL);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawJudgeRapporteur(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_JudgeRapporteur', EXAMPLE_CASELAW_JUDGE_RAPPORTEUR);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawEurovoc(lang) {
    clearSearchBox('CaseLaw');
    if (lang === 'bg') {
        populateTree('Cases_Eurovoc', EXAMPLE_CASELAW_EUROVOC_BG);
    }
    else {
        populateTree('Cases_Eurovoc', EXAMPLE_CASELAW_EUROVOC);
    }
   
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawSubjectMatter(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_SubjectMatter', EXAMPLE_CASELAW_SUBJECT_MATTER);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawDirectoryLaw(lang) {
    clearSearchBox('CaseLaw');
    if (lang === 'bg') {
        populateTree('Cases_DirectoryCaseLaw', EXAMPLE_CASELAW_DIRECTORY_BG);
    }
    else {
        populateTree('Cases_DirectoryCaseLaw', EXAMPLE_CASELAW_DIRECTORY);
    }
  
    DoSearch('tabCases', false);
    return false;
}

function hintExampleCaseLawEuroCasesClassifier(lang) {
    clearSearchBox('CaseLaw');
    populateTree('Cases_EuroCases', EXAMPLE_CASELAW_EUROCASES);
    DoSearch('tabCases', false);
    return false;
}

function hintExampleLegislationDocumentType(lang) {
    clearSearchBox('Legislation');
    populateTree('Law_DocumentTypes', EXAMPLE_LEGISLATION_DOCUMENT_TYPE);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLegislationCelex(lang) {
    clearSearchBox('Legislation');
    $('#Law_NatID_ELI').val(EXAMPLE_LEGISLATION_CELEX);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLegislationOJSeries(lang) {
    $('#select-oj-series option[value=0]').prop('selected', true);
    return false;
}

function hintExampleLegislationDate(lang) {
    clearSearchBox('Legislation');
    $('#Law_DateFrom').val(EXAMPLE_LEGISLATION_DATE);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLegislationDateType(lang) {
    $('#law-date-type-select option[value=2]').prop('selected', true);
    return false;
}

function hintExampleLegislationDirectory(lang) {
    clearSearchBox('Legislation');
    populateTree('Law_DirectoryLegislation', EXAMPLE_LEGISLATION_DIRECTORY);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLegislationEurovoc(lang) {
    clearSearchBox('Legislation');
    populateTree('Law_Eurovoc', EXAMPLE_LEGISLATION_EUROVOC);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLegislationSubjectMatter(lang) {
    clearSearchBox('Legislation');
    populateTree('Law_SubjectMatter', EXAMPLE_LEGISLATION_SUBJECT_MATTER);
    DoSearch('tabLaw', false);
    return false;
}

function hintExampleLogicalExpressions(lang) {
    clearSearchBox('Simple');
    if (lang === 'bg') {
        $("#SearchText").val(EXAMPLE_LOGICAL_EXPRESSIONS_BG);
    }
    else {
        $('#SearchText').val(EXAMPLE_LOGICAL_EXPRESSIONS);
    }

    DoSearch('simple');
}

function clearSearchBox(type) {
    switch (type) {
        case 'CaseLaw': clearAdvSearchFilters('tabCases'); $('#SearchText').val(''); break;
        case 'Legislation': clearAdvSearchFilters('tabLaw'); $('#SearchText').val(''); break;
        case 'Simple': $('#SearchText').val(''); break;
        default: $('#SearchText').val(''); break;
    }
}