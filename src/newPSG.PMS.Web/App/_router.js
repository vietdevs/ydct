//QUAN LY HO SO
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        // QL Hồ Sơ
        {
            //Tra cứu hồ sơ
            {
                $stateProvider.state('tracuucommonhoso', {
                    url: '/tracuucommonhoso',
                    templateUrl: '../App/quanlyhoso/_common/views/traCuuHoSoCommon/traCuuHoSoCommon.html',
                });

                if (abp.auth.hasPermission('Pages.ThuTucDefault.MotCuaPhanCong')) {
                    $stateProvider.state('motcuaphancongchung', {
                        url: '/motcuaphancongchung',
                        templateUrl: '../App/quanlyhoso/_common/views/motCuaPhanCong/motCuaPhanCong.html',
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTucDefault.VanThuDuyet')) {
                    $stateProvider.state('vanthuduyetchung', {
                        url: '/vanthuduyetchung',
                        templateUrl: '../App/quanlyhoso/_common/views/vanThuDuyet/vanThuDuyet.html',
                    });
                }
            }

            //Thủ tục 99
            {
                if (abp.auth.hasPermission('Pages.ThuTuc99.DangKyHoSo')) {
                    $stateProvider.state('tt99/dangkyhoso', {
                        url: '/tt99/dangkyhoso?kp_statuscode',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/1_dangkyhoso/index.cshtml',
                    });
                }

                if (abp.auth.hasPermission('Pages.ThanhToan.XacNhanThanhToan')) {
                    $stateProvider.state('tt99/xacnhanthanhtoan', {
                        url: '/tt99/xacnhanthanhtoan',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/11_xacnhanthanhtoan/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.MotCuaRaSoat')) {
                    $stateProvider.state('tt99/rasoathoso', {
                        url: '/tt99/rasoathoso',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/12_motcuarasoat/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.MotCuaPhanCong')) {
                    $stateProvider.state('tt99/motcuaphancong', {
                        url: '/tt99/motcuaphancong',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/2_motcuaphancong/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.PhongBanPhanCong')) {
                    $stateProvider.state('tt99/phongbanphancong', {
                        url: '/tt99/phongbanphancong',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/21_phongbanphancong/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.ThamDinhHoSo')) {
                    $stateProvider.state('tt99/thamdinhhoso', {
                        url: '/tt99/thamdinhhoso',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/3_thamdinhhoso/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.PhoPhongDuyet')) {
                    $stateProvider.state('tt99/phophongduyet', {
                        url: '/tt99/phophongduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/31_phophongduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.TruongPhongDuyet')) {
                    $stateProvider.state('tt99/truongphongduyet', {
                        url: '/tt99/truongphongduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/4_truongphongduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.LanhDaoCucDuyet')) {
                    $stateProvider.state('tt99/lanhdaocucduyet', {
                        url: '/tt99/lanhdaocucduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/5_lanhdaocucduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.LanhDaoBoDuyet')) {
                    $stateProvider.state('tt99/lanhdaoboduyet', {
                        url: '/tt99/lanhdaoboduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/6_lanhdaoboduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc99.VanThuDuyet')) {
                    $stateProvider.state('tt99/vanthuduyet', {
                        url: '/tt99/vanthuduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/7_vanthuduyet/index.cshtml'
                    });
                }
            }

            // Thủ tục 98
            {
                if (abp.auth.hasPermission('Pages.ThuTuc98.DangKyHoSo')) {
                    $stateProvider.state('tt98/dangkyhoso', {
                        url: '/tt98/dangkyhoso?kp_statuscode',
                        templateUrl: '~/App/quanlyhoso/thutuc98/views/1_dangkyhoso/index.cshtml',
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc98.MotCuaRaSoat')) {
                    $stateProvider.state('tt98/rasoathoso', {
                        url: '/tt98/rasoathoso',
                        templateUrl: '~/App/quanlyhoso/thutuc98/views/12_motcuarasoat/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc98.TruongPhongDuyet')) {
                    $stateProvider.state('tt98/truongphongduyet', {
                        url: '/tt98/truongphongduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc98/views/4_truongphongduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc98.TraCuuHoSo')) {
                    $stateProvider.state('tt98/tracuuhoso', {
                        url: '/tt98/tracuuhoso',
                        templateUrl: '~/App/quanlyhoso/thutuc98/views/tracuuhoso/index.cshtml'
                    });
                }
            }

            // Thủ tục 37
            {
                if (abp.auth.hasPermission('Pages.ThuTuc37.DangKyHoSo')) {
                    $stateProvider.state('tt37/dangkyhoso', {
                        url: '/tt37/dangkyhoso?kp_statuscode',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/1_dangkyhoso/index.cshtml',
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.MotCuaRaSoat')) {
                    $stateProvider.state('tt37/rasoathoso', {
                        url: '/tt37/rasoathoso',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/12_motcuarasoat/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThanhToan.XacNhanThanhToan')) {
                    $stateProvider.state('tt37/xacnhanthanhtoan', {
                        url: '/tt37/xacnhanthanhtoan',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/11_xacnhanthanhtoan/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.MotCuaPhanCong')) {
                    $stateProvider.state('tt37/motcuaphancong', {
                        url: '/tt37/motcuaphancong',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/2_motcuaphancong/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.PhongBanPhanCong')) {
                    $stateProvider.state('tt37/phongbanphancong', {
                        url: '/tt37/phongbanphancong',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/21_phongbanphancong/index.cshtml'
                    });
                }


                if (abp.auth.hasPermission('Pages.ThuTuc37.ThamXetHoSo')) {
                    $stateProvider.state('tt37/thamxethoso', {
                        url: '/tt37/thamxethoso',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/3_thamxethoso/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.ThamDinhHoSo')) {
                    $stateProvider.state('tt37/thamdinhhoso', {
                        url: '/tt37/thamdinhhoso',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/33_hoidongthamdinh/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.TongHopThamDinh')) {
                    $stateProvider.state('tt37/tonghopthamdinh', {
                        url: '/tt37/tonghopthamdinh',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/33_tonghopthamdinh/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.TruongPhongDuyet')) {
                    $stateProvider.state('tt37/truongphongduyet', {
                        url: '/tt37/truongphongduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/4_truongphongduyet/index.cshtml'
                    });
                }
                if (abp.auth.hasPermission('Pages.ThuTuc37.TruongPhongDuyetThamDinh')) {
                    $stateProvider.state('tt37/thamdinhtruongphongduyet', {
                        url: '/tt37/thamdinhtruongphongduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/41_truongphongduyetthamdinh/index.cshtml'
                    });
                }
                if (abp.auth.hasPermission('Pages.ThuTuc37.LanhDaoCucDuyet')) {
                    $stateProvider.state('tt37/lanhdaocucduyet', {
                        url: '/tt37/lanhdaocucduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/5_lanhdaocucduyet/index.cshtml'
                    });
                }
                if (abp.auth.hasPermission('Pages.ThuTuc37.LanhDaoCucDuyetThamDinh')) {
                    $stateProvider.state('tt37/thamdinhlanhdaocucduyet', {
                        url: '/tt37/thamdinhlanhdaocucduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/51_lanhdaocucduyetthamdinh/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.VanThuDuyet')) {
                    $stateProvider.state('tt37/vanthuduyet', {
                        url: '/tt37/vanthuduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/7_vanthuduyet/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.VanThuDuyetThamDinh')) {
                    $stateProvider.state('tt37/thamdinhvanthuduyet', {
                        url: '/tt37/thamdinhvanthuduyet',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/71_vanthuduyetthamdinh/index.cshtml'
                    });
                }

                if (abp.auth.hasPermission('Pages.ThuTuc37.TraCuuHoSo')) {
                    $stateProvider.state('tt37/tracuuhoso', {
                        url: '/tt37/tracuuhoso',
                        templateUrl: '~/App/quanlyhoso/thutuc37/views/tracuuhoso/index.cshtml'
                    });
                }
            }

        }

        $qProvider.errorOnUnhandledRejections(false);
    }
]);

//QUAN LY DANH MUC
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        // QL Danh Mục
        {
            if (abp.auth.hasPermission('Pages.DanhMuc.ChuKy')) {
                $stateProvider.state('chuky', {
                    url: '/chuky',
                    templateUrl: '~/App/danhmuc/views/chukyso/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.LoaiHoSo')) {
                $stateProvider.state('loaihoso', {
                    url: '/loaihoso',
                    templateUrl: '~/App/danhmuc/views/loaihoso/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.LoaiFile')) {
                $stateProvider.state('loaifile', {
                    url: '/loaifile',
                    templateUrl: '~/App/danhmuc/views/loaifile/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.ChucDanh')) {
                $stateProvider.state('chucdanh', {
                    url: '/chucdanh',
                    templateUrl: '~/App/danhmuc/views/chucdanh/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.QuocGia')) {
                $stateProvider.state('quocgia', {
                    url: '/quocgia',
                    templateUrl: '~/App/danhmuc/views/quocgia/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.Tinh')) {
                $stateProvider.state('tinh', {
                    url: '/tinh',
                    templateUrl: '~/App/danhmuc/views/tinh/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.Huyen')) {
                $stateProvider.state('huyen', {
                    url: '/huyen',
                    templateUrl: '~/App/danhmuc/views/huyen/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.Xa')) {
                $stateProvider.state('xa', {
                    url: '/xa',
                    templateUrl: '~/App/danhmuc/views/xa/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.LoaiHinhDoanhNghiep')) {
                $stateProvider.state('loaihinhdoanhnghiep', {
                    url: '/loaihinhdoanhnghiep',
                    templateUrl: '~/App/danhmuc/views/loaihinhdoanhnghiep/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.LichLamViec')) {
                $stateProvider.state('lichlamviec', {
                    url: '/lichlamviec',
                    templateUrl: '~/App/danhmuc/views/lichlamviec/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.PhongBan')) {
                $stateProvider.state('phongban', {
                    url: '/phongban',
                    templateUrl: '~/App/danhmuc/views/phongban/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.DanhMuc.NhomSanPham')) {
                $stateProvider.state('nhomsanpham', {
                    url: '/nhomsanpham',
                    templateUrl: '~/App/danhmuc/views/nhomsanpham/index.cshtml'
                });
            }

            $stateProvider.state('danhsachthutuc', {
                url: '/danhsachthutuc',
                templateUrl: '~/App/danhmuc/views/thutuc/index.cshtml'
            });

            $stateProvider.state('luachonthutuc', {
                url: '/luachonthutuc',
                templateUrl: '~/App/danhmuc/views/thutuc/lua-chon-thu-tuc/lua-chon-thu-tuc.cshtml'
            });

            $stateProvider.state('cuakhau', {
                url: '/cuakhau',
                templateUrl: '~/App/danhmuc/views/cuakhau/index.cshtml'
            });
        }
    }
]);

//QUAN LY DOANH NGHIEP
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        // QL Doanh Nghiệp
        {
            if (abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.SuaThongTinDoanhNghiep')) {
                $stateProvider.state('suathongtindoanhnghiep', {
                    url: '/suathongtindoanhnghiep',
                    templateUrl: '~/App/quanlydoanhnghiep/views/doanhnghiepsuahoso/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.DanhSachDoanhNghiep')) {
                $stateProvider.state('danhsachdoanhnghiep', {
                    url: '/danhsachdoanhnghiep',
                    templateUrl: '~/App/quanlydoanhnghiep/views/danhmucdoanhnghiep/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.DanhSachConDau')) {
                $stateProvider.state('danhsachcondau', {
                    url: '/danhsachcondau',
                    templateUrl: '~/App/quanlydoanhnghiep/views/danhsachcondau/index.cshtml'
                });
            }
        }
    }
]);

//THONG KE BAO CAO
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        // Thống kê báo cáo
        {
            if (abp.auth.hasPermission('Pages.ThongKeBaoCao')) {
                $stateProvider.state('thongkebaocao', {
                    url: '/thongkebaocao',
                    templateUrl: '~/App/thongkebaocao/views/index.cshtml'
                });
            }
            if (abp.auth.hasPermission('Pages.ThongKeBaoCao.MotCua')) {
                $stateProvider.state('thongkebaocaomotcua', {
                    url: '/thongkebaocaomotcua',
                    templateUrl: '~/App/thongkebaocao/views/thongkemotcua.cshtml'
                });
            }
            if (abp.auth.hasPermission('Pages.ThongKeBaoCao.VanThu')) {
                $stateProvider.state('thongkebaocaovanthu', {
                    url: '/thongkebaocaovanthu',
                    templateUrl: '~/App/thongkebaocao/views/thongkevanthu.cshtml'
                });
            }
        }
    }
]);

//WEBSITE
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        if (abp.auth.hasPermission('Pages.Website.BoThuTuc')) {
            $stateProvider.state('bothutuc', {
                url: '/bothutuc',
                templateUrl: '~/App/website/views/bothutuc/index.cshtml'
            });
        }

        if (abp.auth.hasPermission('Pages.Website.ThongBao')) {
            $stateProvider.state('thongbao', {
                url: '/thongbao',
                templateUrl: '~/App/website/views/thongbao/index.cshtml'
            });
        }

        if (abp.auth.hasPermission('Pages.Website.LienHe')) {
            $stateProvider.state('lienhe', {
                url: '/lienhe',
                templateUrl: '~/App/website/views/lienhe/index.cshtml'
            });
        }

        if (abp.auth.hasPermission('Pages.Website.CauHinhChung')) {
            $stateProvider.state('cauhinhchung', {
                url: '/cauhinhchung',
                templateUrl: '~/App/website/views/cauhinhchung/common.cshtml'
            });
        }
    }
]);

//HUONG DAN SU DUNG
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        // Hướng dẫn sử  dụng
        {
            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungDoanhNghiep')) {
                $stateProvider.state('huongdansudungdoanhnghiep', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_DoanhNghiep.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungLanhDaoCuc')) {
                $stateProvider.state('huongdansudunglanhdaocuc', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_LanhDao.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungTruongPhong')) {
                $stateProvider.state('huongdansudungtruongphong', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_TruongPhong.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungPhoPhong')) {
                $stateProvider.state('huongdansudungphophong', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_PhoPhong.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungChuyenVien')) {
                $stateProvider.state('huongdansudungchuyenvien', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_ChuyenVien.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungVanThu')) {
                $stateProvider.state('huongdansudungvanthu', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_VanThu.docx';
                    }
                });
            }

            if (abp.auth.hasPermission('Pages.HDSD.HuongDanSuDungKeToan')) {
                $stateProvider.state('huongdansudungketoan', {
                    onEnter: function ($window) {
                        location.href = '/Temp/HuongDanSuDung/HDSD_ChiCuc_KeToan.docx';
                    }
                });
            }
        }
    }
]);

//QUAN TRI CHUNG
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        //Quản lý Admin
        {
            //Tenant
            if (abp.auth.hasPermission('Pages.QuanLyAdmin')) {
                $stateProvider.state('quanlyadmin', {
                    url: '/quanlyadmin',
                    templateUrl: '~/App/quanlyadmin/views/index.cshtml'
                });
            }
        }
    }
]);

//QUAN LY TAI KHOAN
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        {
            if (abp.auth.hasPermission('Pages.TaiKhoanChuyenGia')) {
                $stateProvider.state('tk-chuyengia', {
                    url: '/tk-chuyengia',
                    templateUrl: '~/App/quanlytaikhoan/chuyengia/index.cshtml'
                });
            }
            if (abp.auth.hasPermission('Pages.TaiKhoanSoYTe')) {
                $stateProvider.state('tk-soyte-vienkn', {
                    url: '/tk-soyte-vienkn',
                    templateUrl: '~/App/quanlytaikhoan/soyte-vkn/index.cshtml'
                });
            }
            if (abp.auth.hasPermission('Pages.TaiKhoanCanBoCuc')) {
                $stateProvider.state('canbocuc', {
                    url: '/canbocuc',
                    templateUrl: '~/App/quanlytaikhoan/canbocuc/index.cshtml'
                });
            }
        }

        $qProvider.errorOnUnhandledRejections(false);
    }
]);

//THIẾT LẬP CHUNG
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        if (abp.auth.hasPermission('Pages.ThietLap.LoaiBienBanThamDinh')) {
            $stateProvider.state('loaibienbanthamdinh', {
                url: '/loaibienbanthamdinh',
                templateUrl: '~/App/thietlap/views/loaibienbanthamdinh/index.cshtml'
            });
        }
        if (abp.auth.hasPermission('Pages.ThietLap.DonViChuyenGia')) {
            $stateProvider.state('donvichuyengia', {
                url: '/donvichuyengia',
                templateUrl: '~/App/thietlap/views/donvichuyengia/index.cshtml'
            });
        }
        if (abp.auth.hasPermission('Pages.ThietLap.PhanLoaiHoSo')) {
            $stateProvider.state('phanloaihoso', {
                url: '/phanloaihoso',
                templateUrl: '~/App/thietlap/views/phanloaihoso/index.cshtml'
            });
        }
        if (abp.auth.hasPermission('Pages.ThietLap.TieuChiThamDinh')) {
            $stateProvider.state('tieuchithamdinh', {
                url: '/tieuchithamdinh',
                templateUrl: '~/App/thietlap/views/tieuchithamdinh/index.cshtml'
            });
        }
        if (abp.auth.hasPermission('Pages.ThietLap.TieuChiThamDinh_LyDo')) {
            $stateProvider.state('tieuchithamdinhlydo', {
                url: '/tieuchithamdinhlydo',
                templateUrl: '~/App/thietlap/views/tieuchithamdinhlydo/index.cshtml'
            });
        }
    }
]);

//HUONG DAN SU DUNG
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        {
            if (abp.auth.hasPermission('Pages.HuongDanSuDung.View')) {
                $stateProvider.state('huongdansudung', {
                    url: '/huongdansudung',
                    templateUrl: '~/App/chuyenmucbaiviet/huongdansudung/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.HuongDanSuDung.DanhMuc')) {
                $stateProvider.state('huongdansudung_chuyenmuc', {
                    url: '/huongdansudung_chuyenmuc',
                    templateUrl: '~/App/chuyenmucbaiviet/huongdansudung/category/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.HuongDanSuDung.BaiViet')) {
                $stateProvider.state('huongdansudung_baiviet', {
                    url: '/huongdansudung_baiviet',
                    templateUrl: '~/App/chuyenmucbaiviet/huongdansudung/article/index.cshtml'
                });
            }
        }
        $qProvider.errorOnUnhandledRejections(false);
    }
]);

//QUAN TRI DU AN
appModule.config([
    '$stateProvider', '$urlRouterProvider', '$qProvider',
    function ($stateProvider, $urlRouterProvider, $qProvider) {
        {
            if (abp.auth.hasPermission('Pages.QuanTriDuAn.View')) {
                $stateProvider.state('quantriduan', {
                    url: '/quantriduan',
                    templateUrl: '~/App/chuyenmucbaiviet/quantriduan/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.QuanTriDuAn.DanhMuc')) {
                $stateProvider.state('quantriduan_chuyenmuc', {
                    url: '/quantriduan_chuyenmuc',
                    templateUrl: '~/App/chuyenmucbaiviet/quantriduan/category/index.cshtml'
                });
            }

            if (abp.auth.hasPermission('Pages.QuanTriDuAn.BaiViet')) {
                $stateProvider.state('quantriduan_baiviet', {
                    url: '/quantriduan_baiviet',
                    templateUrl: '~/App/chuyenmucbaiviet/quantriduan/article/index.cshtml'
                });
            }
        }
        $qProvider.errorOnUnhandledRejections(false);
    }
]);