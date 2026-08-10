
    // Cache data - chỉ load từ server 1 lần duy nhất
    let allSanPhamList = [];

    // Load danh sách khi trang load
    window.addEventListener('DOMContentLoaded', async function () {
        await GetListCombo();
    await GetListCategory();
    await GetListPublisher();
    await LoadDataFromServer();
        });

    function ClearFilters() {
        document.getElementById('search-keyword').value = '';
    document.getElementById('combo-id').value = '';
    document.getElementById('category-id').value = '';
    document.getElementById('publisher-id').value = '';
    document.getElementById('filter-status').value = '';
    RenderSanPhamList();
        }

    // Load data từ server 1 lần duy nhất
    async function LoadDataFromServer() {
            const tbody = document.getElementById('sanpham-tbody');
    tbody.innerHTML = '<tr><td colspan="7" class="no-data">Đang tải dữ liệu...</td></tr>';

    try {
                const resultText = await PostJSON('/SanPham/GetAllSanPham', { });
    allSanPhamList = JSON.parse(resultText);

    if (!allSanPhamList || allSanPhamList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="no-data">Không có sản phẩm nào.</td></tr>';
    return;
                }

    // Render lần đầu
    RenderSanPhamList();
            } catch (error) {
        tbody.innerHTML = '<tr><td colspan="8" class="no-data" style="color: red;">Lỗi tải dữ liệu: ' + error.message + '</td></tr>';
            }
        }

    // Render danh sách (chỉ filter từ cached data, KHÔNG gọi server)
    function RenderSanPhamList() {
            const tbody = document.getElementById('sanpham-tbody');

    if (!allSanPhamList || allSanPhamList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="no-data">Không có sản phẩm nào.</td></tr>';
    return;
            }

    // Lọc theo các điều kiện
    const keyword = document.getElementById('search-keyword').value.trim().toLowerCase();
    const comboName = document.getElementById('combo-id').value.trim();
    const categoryName = document.getElementById('category-id').value.trim();
    const publisherName = document.getElementById('publisher-id').value.trim();
    const statusValue = document.getElementById('filter-status').value;

    // Lấy ID từ datalist
    const comboId = comboName ? GetDataIdFromComboDatalist(comboName) : null;
    const categoryId = categoryName ? GetDataIdFromCategoryDatalist(categoryName) : null;
    const publisherId = publisherName ? GetDataIdFromPublisherDatalist(publisherName) : null;

            let filteredList = allSanPhamList.filter(sp => {
                // Filter by keyword
                if (keyword) {
                    const name = (sp.Name || '').toLowerCase();
    const shortName = (sp.ShortName || '').toLowerCase();
    const code = (sp.Code || '').toLowerCase();
    const barcode = (sp.Barcode || '').toLowerCase();
    const matchKeyword = name.includes(keyword) ||
    shortName.includes(keyword) ||
    code.includes(keyword) ||
    barcode.includes(keyword);
    if (!matchKeyword) return false;
                }

    // Filter by ComboId
    if (comboId !== null && comboId !== -1) {
                    if (sp.ComboId !== comboId) return false;
                }

    // Filter by CategoryId
    if (categoryId !== null && categoryId !== -1) {
                    if (sp.CategoryId !== categoryId) return false;
                }

    // Filter by PublisherId
    if (publisherId !== null && publisherId !== -1) {
                    if (sp.PublisherId !== publisherId) return false;
                }

    // Filter by Status
    if (statusValue !== '') {
                    if (sp.Status !== parseInt(statusValue)) return false;
                }

    return true;
            });

    if (filteredList.length === 0) {
        tbody.innerHTML = '<tr><td colspan="8" class="no-data">Không tìm thấy sản phẩm nào.</td></tr>';
    return;
            }

    // Render table rows
    tbody.innerHTML = '';
            filteredList.forEach(sp => {
                const row = document.createElement('tr');
    row.innerHTML = `
    <td>${sp.Id}</td>
    <td>${sp.Name || ''}</td>
    <td>${sp.ShortName || ''}</td>
    <td>${FormatCurrency(sp.BookCoverPrice || 0)}</td>
    <td>${sp.Discount || 0}%</td>
    <td>${sp.Quantity || 0}</td>
    <td>${FormatStatus(sp.Status)}</td>
    <td>
        <a href="/SanPham/UpdateDelete?id=${sp.Id}" class="edit-link" target="_blank" rel="noopener noreferrer">Chỉnh sửa</a>
    </td>
    `;
    tbody.appendChild(row);
            });
        }

    function FormatCurrency(amount) {
            return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
    currency: 'VND'
            }).format(amount);
        }

    function FormatStatus(status) {
            switch (status) {
                case 0: return '<span style="color: #4CAF50;">✓ Đang kinh doanh</span>';
    case 1: return '<span style="color: #FF9800;">⚠ Tạm hết hàng</span>';
    case 2: return '<span style="color: #f44336;">✗ Ngừng kinh doanh</span>';
    default: return '<span style="color: #999;">Không xác định</span>';
            }
        }
