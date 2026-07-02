window.onload = async function () {
    await GetListProductName();
    await GetListCombo();
    await GetListCategory();
    await InitializeModal();
};
