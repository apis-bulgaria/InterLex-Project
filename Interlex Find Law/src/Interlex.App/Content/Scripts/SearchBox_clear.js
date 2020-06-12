function clearCasesFilters(keepMultiDictInformation) {
    //$("#formCases :input[id^='b'][id$='Clear']:button").click();
    $('#SearchText').val('');

    $('#formCases .btn-clear').click();
    $('#Cases_NatID_ECLI').val('');
    $('#Cases_Parties').val('');
    $('#Cases_DateFrom').val('');
    $('#Cases_DateTo').val('');

    $('#Cases_CaseNumber').val('');
    $('#Cases_Year').val('');
    $('#Cases_DocumentType').val('');
    $('#Cases_ProcedureType').val('');
    $('#Cases_GeneralLawyer').val('');
    $('#Cases_JudgeRapporteur').val('');
    $('#Cases_Enactment').val('');
    $('#Cases_EnactmentCelex').val('');
    $('#Cases_Provision').val('').trigger('change');
    $('#Cases_ProvisionParOriginal').val('').trigger('change');

    $('.eu-specific').hide();

    $('#container-cases-parties').addClass('width-50');
    $('#container-cases-parties').removeClass('width-25');
    $('#container-cases-case').hide();
    $('#container-cases-casenumber').hide();
    $('#container-cases-year').hide();

    $('#Cases_OnlyInTitles').prop('checked', false);
    $('#Cases_DirectoryCaseLawFull').prop('checked', false);

    $('#tbCases_CourtsFoldersText').val('');
    clearTreeFilter('Cases_CourtsFolders');
    $('#tbCases_DirectoryCaseLawText').val('');
    clearTreeFilter('Cases_DirectoryCaseLaw');
    $('#tbCases_EuroCasesText').val('');
    clearTreeFilter('Cases_EuroCases');
    $('#tbCases_EurovocText').val('');
    clearTreeFilter('Cases_Eurovoc');
    $("#tbCases_DocumentTypesText").val('');
    clearTreeFilter('Cases_DocumentTypes');
    $('#tbCases_ProcedureTypeText').val('');
    clearTreeFilter('Cases_ProcedureType');
    $('#tbCases_AdvocateGeneralText').val('');
    clearTreeFilter('Cases_AdvocateGeneral');
    $('#tbCases_JudgeRapporteurText').val('');
    clearTreeFilter('Cases_JudgeRapporteur');
    $('#tbCases_StatesText').val('');
    clearTreeFilter('Cases_States');
    $('#tbCases_CourtsText').val('');
    clearTreeFilter('Cases_Courts');
    $('#tbCases_HudocImportanceText').val('');
    clearTreeFilter('Cases_HudocImportance');
    $('#tbCases_HudocArticleViolationText').val('');
    clearTreeFilter('Cases_HudocArticleViolation');
    $('#tbCases_HudocApplicabilityText').val('');
    clearTreeFilter('Cases_HudocApplicability');
    $('#tbCases_HudocText').val('');
    clearTreeFilter('Cases_Hudoc');

    if ($('#tbCases_HudocArticlesText').length) {
        clearTreeFilter('Cases_HudocArticles');
    }

    if ($('#tbCases_RulesOfTheCourtText').length) {
        clearTreeFilter('Cases_RulesOfTheCourt');
    }

    $('#refered-act-provision-input-container').find('input').val('');
    $('#input-refered-act').val('');
    $('#Cases_ECHRReferedActType').val(0); // Not selected refered type
    $('#Cases_ReferedActECHRDocLangId').val(''); // Clearing hidden for chosen refered act
    $('#Cases_ReferedActTitle').val(''); // Clearing hidden for chosen refered act title

    $('#Cases_EnactmentText').val('');
    $('#Cases_EnactmentDocLangId').val('');
    $('#Cases_ProvisionText').val('');
    $('#Cases_ProvisionId').val('');

    clearEnactmentProvision('Cases');

    $('#Cases_Applicant').val('');
    $('#Cases_ApplicationNumber').val('');

    if ($('#tbCases_SyllabusText').length) {
        $('#tbCases_SyllabusText').val('');
        clearTreeFilter('Cases_Syllabus');
    }

    $('#tbCases_SubjectMatterText').val('');
    clearTreeFilter('Cases_SubjectMatter');

    $(':radio[name="Cases.CaseLawType"][value=0]').trigger('click');

    /*  $(':radio[name="Cases.CaseLawType"][value=0]').prop('checked', 'checked');
      $('input[name="Cases.CaseLawType"]:not(:checked)').removeAttr('checked');
      $(':radio[name="Cases.CaseLawType"][value=0]').attr('checked', 'checked');*/

    $(':radio[name="Cases.DatePeriodType"][value=date]').prop('checked', 'checked');
    $('#Cases_DateTo').prop('disabled', 'disabled');

    if (keepMultiDictInformation !== true) {
        multiDictClear();
    }

    $('#link-choose-enactment-index').val('');
    $('#link-choose-enactment-index').attr('title', '');

    $('.clear-link').hide();
}

function clearEnactmentProvision(type) {
    $('#' + type + '_EnactmentText').val('');
    $('#' + type + '_EnactmentDocLangId').val('');
    $('#' + type + '_ProvisionText').val('');
    $('#' + type + '_ProvisionId').val('');
}

function clearLawFilters() {
    $('#formLaw .btn-clear').click();
    $('#SearchText').val('');
    $('#Law_NatID_ELI').val('');
    $('#Law_Year').val('');
    $('#Law_Month').val('');
    $('#Law_Day').val('');
    $('#Law_Number').val('');
    $('#Law_PageNumber').val('');
    $('#Law_DocNumber').val('');
    $('#Law_OJYear').val('');

    $("#Law_OnlyInActualActs").prop('checked', false);
    $('#Law_OnlyInBasicActs').prop('checked', false);

    $('#select-oj-series option[value="-1"]').prop('selected', true);
    $('#law-date-type-select option[value="0"]').prop('selected', true);

    $(':radio[name="Law.LegislationType"][value=1]').trigger('click');

    $('#Law_OnlyInTitles').prop('checked', false);

    var lawDateTypeDropdownDefaultValue = $('#law-date-type-dropdown span').first().text();
    $('#span-law-current-datetype').text(lawDateTypeDropdownDefaultValue);
    $('#Law_LawDateType').val(0);

    $('#tbLaw_DirectoryLegislation').val('');
    clearTreeFilter('Law_DirectoryLegislation');
    $('#tbLaw_EurovocText').val('');
    clearTreeFilter('Law_Eurovoc');
    $('#tbLaw_SubjectMatterText').val('');
    clearTreeFilter('Law_SubjectMatter');
    $('#tbLaw_DocumentTypesText').val('');
    clearTreeFilter('Law_DocumentTypes');
    $('#tbLaw_ActJurisdictions').val('');
    clearTreeFilter('Law_ActJurisdictions');

    if ($('#tbLaw_JurisdictionText').length) {
        $('#tbLaw_JurisdictionText').val('');
        clearTreeFilter('Law_Jurisdiction');
    }

    if ($('#tbLaw_SyllabusText').length) {
        $('#tbLaw_SyllabusText').val('');
        clearTreeFilter('Law_Syllabus');
    }


    //if (keepMultiDictInformation !== true) { // consider implementing this again for a specific behaviour
     multiDictClear();
   // }
    

    $('.clear-link').hide();
}

function clearFinancesFilters() {
    $('#SearchText').val('');

    // clearing checkboxes
    $('#ExactMatch').prop('checked', false);
    $('#Finances_Keywords').prop('checked', false);
    $('#Finances_OnlyInTitles').prop('checked', false);
    $('#Finances_SearchInSummaries').prop('checked', false);

    // clearing document type checkboxes
    $('#finances-document-type-container label input[type="checkbox"]').prop('checked', false);
    $('#finances-document-type-tb').val('');

    clearEnactmentProvision('Finances');

    // clearing dates
    $('#Finances_DateFrom').val('');
    $('#Finances_DateTo').val('');
    $('#Finances_DateTo').prop('disabled', true);
    $(':radio[name="Finances.DatePeriodType"][value=date]').prop('checked', 'checked');

    // clearing tree classifiers
    $('#tbFinances_EuroFinanceText').val('');
    clearTreeFilter('Finances_EuroFinance');

    $('.clear-link').hide();
}

function clearAdvSearchFilters(activeTab) {
    //activeTab = $("#AdvSearchTabs").tabs("option", "active");
    switch (activeTab) {
        case 'tabCases': // Cases
            clearCasesFilters();
            $(".add-clear-span a").hide();
            break;
        case 'tabLaw': // Law
            clearLawFilters();
            $('.add-clear-span a').hide();
            break;
        case 'tabFinances':
            clearFinancesFilters();
            $('.add-clear-span a').hide();
            break;
    }

    $('.eucs-clear').css('visibility', 'hidden');
}