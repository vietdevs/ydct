var app = app || {};
app.defaultPageSize = 20;
app.keyThuTuc = "Thủ tục hành chính";
app.dateRangeDefault = {
    startDate: new Date(new Date().getFullYear(), new Date().getMonth() - 1, new Date().getDate()),
    endDate: new Date(),
}
app.dateDefault = {
    startDate: null,
    endDate: null,
}
app.pageable = {
    pageSizes: [10, 20, 50, 100],
    messages: {
        empty: "Không có dữ liệu",
        display: "Hiện thị từ {0} đến {1}/ {2} bản ghi",
        itemsPerPage: "Bản ghi hiển thị"
    }
}