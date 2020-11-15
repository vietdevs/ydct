(function () {
    appModule.controller('quanlyhoso.common.excel.ViewPDF', [
        '$scope', '$uibModalInstance', 'modalData',
        function ($scope, $uibModalInstance, modalData) {
            var vm = this;
            vm.modalTitle = modalData.modalTitle;
            var pdfDto = {
                controller: modalData.controller,
                action: modalData.action,
                hoSoId: modalData.hoSoId,
                noiDungCV: modalData.noiDungCV,
                trangthaiCV: modalData.trangthaiCV,
                hoSoIsDat: modalData.hoSoIsDat,

                trangThaiHoSo: modalData.trangThaiHoSo,
                jsonThamDinhCoSo: modalData.jsonThamDinhCoSo,
                headerCV: modalData.headerCV,
                footerCV: modalData.footerCV,
                loaiHinhDangKyPhuHop: modalData.loaiHinhDangKyPhuHop,
                trangThaiCV: modalData.trangThaiCV,
                tenCoSo: modalData.tenCoSo,
                diaChiCoSo: modalData.diaChiCoSo,
                yKienBoSung: modalData.yKienBoSung,
                isXemTruoc: modalData.isXemTruoc,

                soCongVan: modalData.soCongVan,
                ngayCongVan: modalData.ngayCongVan,
                tenNguoiDaiDien: modalData.tenNguoiDaiDien,
                soDienThoai: modalData.soDienThoai,
                email: modalData.email
            }
            console.log(pdfDto, "pdfDto");
            $("#loadHtml").scrollTop();
            $.ajax({
                type: 'POST',
                url: '/Home/ViewPDF',
                //headers: headers,
                data: pdfDto,
                dataType: "html",
                success: function (data) {
                    $('#loadHtml').html(data);
                },
                error: function (xhr) {
                },
                complete: function () {
                    //UIBlockUI.stopLoadingPage();
                }
            });

            vm.close = function () {
                $uibModalInstance.dismiss();
            }
        }
    ]);
})();