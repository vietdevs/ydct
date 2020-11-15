(function () {
    appModule.factory('quanlyhoso.thutuc37.services.appChuKySo', ['$uibModal',
        function ($uibModal) {

            //THU_TUC_ID
            var thuTucId = 37;
            //MA_THU_TUC
            var maThuTuc = app.MA_THU_TUC.THU_TUC_37;

            var xemFilePDF = function (pathPDF, title) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/modal/viewFilePDFModal.cshtml',
                    controller: 'quanlyhoso.common.modal.viewFilePDFModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            pathPDF: pathPDF,
                            title: title
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                });
            };

            var xemBienBanThamDinh = function (item, fnCallBack) {

                var _pathPDF = "/Report37/TemplateBienBanThamDinh?hoSoId=" + item.id + "#zoom=70";

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/modal/viewFilePDFModal.cshtml',
                    controller: 'quanlyhoso.common.modal.viewFilePDFModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            pathPDF: _pathPDF,
                            title: "Biên bản thẩm định",
                            isTemplate: true
                        }
                    }
                });
            };

            var xemTruocBienBanThamDinh = function (item, fnCallBack) {

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Biên bản thẩm định',
                            controller: "Report37",
                            action: "GoToViewBienBanThamDinh",
                            hoSoId: item.id,
                            noiDungThamXetJson: item.noiDungThamXetJson,
                            yKienChung: item.yKienChung
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            var xemTruocCongVan = function (item, fnCallBack) {

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Giấy công văn',
                            controller: "Report37",
                            action: "GoToViewCongVan",
                            hoSoId: item.id,
                            noiDungCV: item.noiDungCV,
                            hoSoIsDat: item.hoSoIsDat,
                            noiDungThamXetJson: item.noiDungThamXetJson
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            var xemTruocCongVanDat = function (item, fnCallBack) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Công văn hồ sơ đạt',
                            controller: "Report37",
                            action: "GoToViewCongVanHoSoDat",
                            hoSoId: item.hoSoId,
                            soCongVan: item.soCongVan,
                            ngayCongVan: item.ngayCongVan,
                            noiDungCV: item.noiDungCV,
                            tenNguoiDaiDien: item.tenNguoiDaiDien,
                            diaChiCoSo: item.diaChiCoSo,
                            soDienThoai: item.soDienThoai,
                            email: item.email
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            var xemTruocCongVanTuChoi = function (item, fnCallBack) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Công văn từ chối hồ sơ',
                            controller: "Report37",
                            action: "GoToViewCongVanHoSoTuChoi",
                            hoSoId: item.hoSoId,
                            soCongVan: item.soCongVan,
                            ngayCongVan: item.ngayCongVan,
                            noiDungCV: item.noiDungCV,
                            tenNguoiDaiDien: item.tenNguoiDaiDien,
                            diaChiCoSo: item.diaChiCoSo,
                            soDienThoai: item.soDienThoai,
                            email: item.email
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };

            var xemTruocCongVanBoSung = function (item, fnCallBack) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/excel/ViewPDF.cshtml',
                    controller: 'quanlyhoso.common.excel.ViewPDF as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: {
                            modalTitle: 'Công văn yêu cầu bổ sung',
                            controller: "Report37",
                            action: "GoToViewCongVanBoSung",
                            hoSoId: item.hoSoId,
                            soCongVan: item.soCongVan,
                            ngayCongVan: item.ngayYeuCauBoSung,
                            noiDungYeuCauGiaiQuyet: item.noiDungYeuCauGiaiQuyet,
                            noiDungCV: item.noiDungCV,
                            lyDo: item.lyDo,
                            tenCanBoHoTro: item.tenCanBoHoTro,
                            dienThoaiCanBo: item.dienThoaiCanBo,
                            tenNguoiDaiDien: item.tenNguoiDaiDien,
                            diaChiCoSo: item.diaChiCoSo,
                            soDienThoai: item.soDienThoai,
                            email: item.email
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    if (fnCallBack) {
                        fnCallBack(item);
                    }
                });
            };


            //Ký số

            var insertPdfHoSo = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report37/InsertPDFHoSo',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            let data = kq.result;
                            if (fnCallBack) {
                                param = {
                                    hoSoId: dataItem.id,
                                    duongDanTep: data.fileName
                                };

                                fnCallBack(param);
                            }
                            return true;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var kyPhieuTiepNhan = function (dataItem, fnCallBack) {
                if (dataItem.hoSoId > 0) {
                    $.ajax({
                        url: '/Report37/InsertPDFPhieuTiepNhan',
                        type: 'POST',
                        async: false,
                        data: {
                            input: {
                                hoSoId: dataItem.hoSoId,
                                ngayHenCap: dataItem.ngayHenCap,
                                hinhThucCapChungChi: dataItem.hinhThucCapChungChi,
                                listTaiLieuDaNhan: dataItem.listTaiLieuDaNhan,
                            }
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var signedFileName = signUSB(data.fileName);

                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }

                            if (fnCallBack) {
                                param = {
                                    duongDanTep: signedFileName
                                };

                                fnCallBack(param);
                            }

                            return true;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var kySoCongVan = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report37/InsertPDFCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            input: {
                                hoSoId: dataItem.id,
                                hoSoIsDat: dataItem.hoSoIsDat
                            }
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var signedFileName = signUSB(data.fileName);

                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }

                            if (fnCallBack) {
                                param = {
                                    hoSoId: dataItem.id,
                                    hoSoXuLyId: dataItem.hoSoXuLyId_Active,
                                    duongDanTep: signedFileName,
                                    soTiepNhan: data.soTiepNhan
                                };

                                fnCallBack(param);
                            }

                            return true;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var kySoGiayTiepNhan = function (dataItem, fnCallBack) {
                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report37/InsertPDFCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.fileName.length == 0) {
                                return false;
                            }
                            var pathCongVanCA = "";
                            var pathGiayTiepNhanCA = "";

                            var signedFileName = signUSB(data.fileName);
                            //signedFileName err
                            if (signedFileName.length == 0 || signedFileName == undefined) {
                                alert('Ký điện tử thất bại');
                                return false;
                            }
                            pathCongVanCA = angular.copy(signedFileName);
                            //ký thành công tạo ra giay tiếp nhận tổng hợp
                            $.ajax({
                                url: '/Report37/InsertPDFGiayTiepNhan',
                                type: 'POST',
                                async: false,
                                data: {
                                    hoSoId: dataItem.id
                                },
                                success: function (kq) {
                                    var data = kq.result;
                                    if (data.fileName.length == 0) {
                                        return false;
                                    }
                                    var signedFileName = data.fileName;
                                    var soTiepNhan = data.soTiepNhan;

                                    signedFileName = signUSB(signedFileName);
                                    if (signedFileName.length == 0 || signedFileName == undefined) {
                                        alert('Ký điện tử thất bại');
                                        return false;
                                    }
                                    pathGiayTiepNhanCA = angular.copy(signedFileName);

                                    if (fnCallBack) {
                                        param = {
                                            hoSoId: dataItem.id,
                                            hoSoXuLyId: dataItem.hoSoXuLyId_Active,
                                            duongDanTep: pathCongVanCA,
                                            giayTiepNhanCA: pathGiayTiepNhanCA,
                                            soTiepNhan: soTiepNhan
                                        };

                                        fnCallBack(param);
                                    }

                                    return true;
                                }
                            });

                            return false;
                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };

            var vanThuDongDau = function (dataItem, fnCallBack) {

                if (dataItem.id > 0) {
                    $.ajax({
                        url: '/Report37/GetDuongDanTepGiayCongVan',
                        type: 'POST',
                        async: false,
                        data: {
                            hoSoId: dataItem.id
                        },
                        success: function (kq) {
                            var data = kq.result;
                            if (data.status == 1) {

                                if (data.fileName.length == 0) {
                                    return false;
                                }

                                var signedFileName = signUSB(data.fileName);

                                //signedFileName err
                                if (signedFileName.length == 0 || signedFileName == undefined) {
                                    alert('Ký điện tử thất bại');
                                    return false;
                                }

                                if (fnCallBack) {
                                    var param = {
                                        duongDanTep: signedFileName
                                    };

                                    fnCallBack(param);
                                }
                                //}
                                return true;
                            }

                        }
                    });
                }
                else {
                    console.log(dataItem, 'dataItem err');
                }
            };
            //End - Ký số

            return {
                THU_TUC_ID: thuTucId,
                MA_THU_TUC: maThuTuc,
                xemFilePDF,
                xemTruocCongVan,
                xemTruocCongVanDat,
                xemTruocCongVanTuChoi,
                xemTruocCongVanBoSung,
                xemBienBanThamDinh,
                xemTruocBienBanThamDinh,
                insertPdfHoSo,
                kyPhieuTiepNhan,
                kySoCongVan,
                kySoGiayTiepNhan,
                vanThuDongDau
            };
        }
    ]);
})();