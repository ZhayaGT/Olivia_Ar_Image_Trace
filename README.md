# TraceAR: Mobile Augmented Reality untuk Mural Tracing

TraceAR adalah aplikasi berbasis *Mobile Augmented Reality* (AR) yang dirancang untuk mendemokratisasi seni mural. Aplikasi ini membantu ilustrator pemula dan komunitas masyarakat (seperti Karang Taruna) memindahkan karya digital dari perangkat gawai ke media dinding fisik berskala besar secara praktis, presisi, dan hemat biaya tanpa membutuhkan alat proyektor atau perhitungan matematis sistem grid manual yang rumit.

---

## 📌 CATATAN PENTING (PROJECT SCENE LOCATION)

Repositori ini dikembangkan dengan memodifikasi *sample project* resmi dari **Unity AR Foundation**. Seluruh aset utama, skrip koding spesifik, prefab, dan scene operasional aplikasi TraceAR terletak pada direktori berikut:

📂 **`Assets/Scenes/ImageTracking/Jonathan`**

*Pastikan Anda membuka scene di dalam folder tersebut untuk menjalankan atau memodifikasi fungsionalitas utama aplikasi TraceAR.*

---

## 🚀 Fitur Utama Aplikasi

1. **Enhanced Vertical Plane Detection (Metode 4-Stiker Fisik)**
   * Mengatasi keterbatasan sensor kamera HP kelas menengah ke bawah (*low-to-midrange*) pada dinding polos tanpa tekstur (*featureless wall*).
   * Cukup menempelkan 4 stiker penanda kontras tinggi di sudut batas luar dinding untuk menyuntikkan *feature points* buatan. ARCore akan mendeteksi bidang vertikal secara instan dan stabil.

2. **Smart Canvas Calibration & Spasial Transform**
   * Antarmuka kontrol UI (*sliders*) untuk melakukan kalibrasi gambar secara langsung di layar HP.
   * Fitur manipulasi meliputi: Pergeseran posisi (*Offset X & Y*), Ukuran (*Scale*), Rotasi (*Rotate Z*), Transparansi (*Intensity/Alpha*), serta Pembalikan Gambar (*Flip Horizontal & Vertical*).
   * Dilengkapi sistem pencegahan *Z-Fighting* (gambar berkedip) dengan jarak aman otomatis 5 milimeter di depan koordinat dinding.

3. **Dual Image Ingestion**
   * Menyediakan katalog sketsa bawaan aplikasi serta fitur impor file gambar kustom (.png/.jpg) langsung dari galeri lokal penyimpanan ponsel penguna menggunakan integrasi *NativeGallery API*.

4. **Anti-Drift Lock Mode**
   * Mengunci posisi koordinat kanvas digital secara absolut di ruang nyata (*World Space*).
   * Saat masuk ke mode *Tracing*, sistem otomatis menonaktifkan komponen `ARPlaneManager` dan menyembunyikan jaring dinding visual untuk menghemat baterai dan beban kerja CPU perangkat, memastikan performa konstan pada **60 FPS** saat pelukis melakukan penjiplakan dari jarak dekat.

---

## 🛠️ Spesifikasi Teknologi

* **Game Engine:** Unity LTS 2022.3.x
* **Graphics Pipeline:** Universal Render Pipeline (URP)
* **SDK / Framework:** Unity AR Foundation (v5.1.x) & Google ARCore XR Plugin
* **Bahasa Pemrograman:** C# (*Event-Driven Architecture*)
* **Input System:** Unity Input System package (v1.7.x)
* **Target Platform:** Android OS 8.0 (Oreo) ke atas / API Level 26+ (ARM64 Architecture)

---

## 📐 Arsitektur Kode Utama

Logika aplikasi dikendalikan secara modular dan *reactive* oleh tiga skrip utama tanpa membebani fungsi `Update()` bawaan Unity:

* **`MuralPlaneTrackerManager.cs`**: Mengelola deteksi bidang vertikal spasial, memproses *raycasting* sentuhan layar, dan menginisialisasi pembuatan objek kanvas (*MuralRoot*) di dunia nyata.
* **`MuralCanvasController.cs`**: Melekat pada prefab kanvas gambar untuk mengatur material tekstur menggunakan `MaterialPropertyBlock` (bebas bocor memori RAM) serta kalkulasi transformasi matematika spasial gambar.
* **`MuralUIManager.cs`**: Mengendalikan transisi 4 tahapan jendela aplikasi (*State/Window Management*) dari fase pemindaian hingga fase penjiplakan mandiri.

---

## 📋 Alur Operasional Pengguna (App Workflow)
