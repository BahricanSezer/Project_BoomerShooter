The Velocitas - Oyun için Kod Yazımı II Final Projesi

Bu proje, Oyun için Kod Yazımı II dersi final ödevi kapsamında Unity 6 (ProBuilder) kullanılarak geliştirilmiş, hızlı tempoya dayalı (fast-paced) bir Boomer Shooter / Parkur FPS prototipidir. Oyunda amaç, yüksek mobilite mekaniklerini kullanarak engelleri aşmak, düşmanları temizlemek ve bölüm sonundaki portala ulaşmaktır.

1. Oyunun Temel Fikri
The Velocitas, oyuncunun sürekli hareket halinde kalmasını zorunlu kılan bir oynanış döngüsüne sahiptir. Oyuncu durduğu an düşman projectile'larına hedef olacağı için haritada sürekli Slide, Dash, Wallrun ve Grappling Hook mekanikleriyle akıcı bir şekilde hareket etmeli, yol üstündeki silahları toplayarak arenalardaki encounter sekanslarını temizlemelidir.

2. Kullanılan Temel Sistemler
Karakter Hareket Sistemi: Klasik WASD hareketinin yanında ivmeli kayma (Slide), kısa mesafeli atılma (Dash) ve dikey platform geçişleri için özel tasarlanmış Jumppad sistemleri mevcuttur.

Parkur Sistemi: Belirli yüzeylerde çalışan Wallrun mekaniği ve haritadaki uçurumları aşmayı sağlayan fizik tabanlı Grappling Hook (Kanca) sistemi entegre edilmiştir.

Silah ve Atış Sistemi: Raycast tabanlı hitscan vuruş algılaması, collision bazlı efektler ve farklı ateş etme ritimlerine sahip 3 çeşit silah progresyonu (Tabanca, Pompalı, Otomatik) bulunmaktadır.

Checkpoint & Can Sistemi: Seviye geneline yerleştirilen modüler Checkpoint yapıları ve Can Küresi (Health Orb) alma mekanikleri mevcuttur.

3. Coroutine Hangi Amaçla Kullanıldı?
Projede Coroutine yapıları, performansı optimize etmek ve Update fonksiyonunun yükünü hafifletmek amacıyla zamanlama gerektiren ekran efektlerinde kullanılmıştır. Oyuncu haritadan can küresi (Health Orb) toplayıp canını yenilediğinde ekranda anlık beliren yeşil ekran efekti (Heal Volume) tetiklenir. Bu efektin belirlenen süre kadar ekranda kalıp ardından otomatik kapanması işlemi, Update içinde sürekli her karede çalışan bir timer yerine, HealFlashRoutine isimli bir Coroutine yapısıyla asenkron olarak yönetilmiştir.

4. Object Pooling Hangi Yapıda Kullanıldı?
Silahların ateşlenmesi esnasında sürekli Instantiate ve Destroy fonksiyonlarını çağırarak işlemciyi yormamak adına Impact Effect (Vuruş Efekti) ve Projectile sisteminde Object Pooling mimarisi kurulmuştur. Mermilerin duvarlara veya düşmanlara çarptığı anlarda oluşan efekt prefabları (Impact Prefabs) oyun başında ImpactPoolManager tarafından havuzda oluşturulur, mermi patladığında aktif edilir ve işi bittiğinde yok edilmek yerine tekrar havuza pasif olarak geri gönderilen bir yapıda kullanılmıştır.

5. ScriptableObject Hangi Verileri Yönetmektedir?
Oyundaki veri karmaşasını önlemek ve modülerliği sağlamak adına Silah Verileri (Weapon Data) ScriptableObject ile yönetilmektedir. Her silahın;

Hasar miktarı (Damage),

Ateş etme hızı (Fire Rate),

Silahın menzili ve şarjör verileri,

ScriptableObject assetleri üzerinde tutulur. Böylece yeni bir silah eklemek veya mevcut silahların dengesini (Balancing) değiştirmek istendiğinde koda dokunmadan editör üzerinden kolayca veri güncellemesi yapılabilmektedir.

6. PlayerPrefs ile Hangi Veri Saklanmaktadır?
Oyuncunun deneyimini özelleştiren ve oyundan çıkıp girdiğinde kaybolmaması gereken Ayarlar ve Kalıcı Durumlar PlayerPrefs kullanılarak saklanmaktadır:

Ses ayarları Master,Music,SFX volume sistemleri PlayerPrefs ile yapılmıştır. Pause Menudeki sliderlar ile ses kontrol edilir , PlayerPrefs ile kaydedilir.

7. Projede Karşılaşılan Temel Problemler ve Bunlara Getirilen Çözümler

Ölüm Sonrası UI Güncelleme Hatası: Oyuncu elenip son noktada yeniden doğduğunda (Respawn), arka planda can değeri 100 olmasına rağmen ekrandaki UI yazısı 0 HP olarak takılı kalıyordu. Çözüm olarak; ResetHealthToMax() fonksiyonunun hemen ardına küçük bir hasar-şifa kontrol tetikleyicisi eklenerek can arayüzünü güncelleyen fonksiyonların sistem tarafından zorla uyarılması sağlanmış ve arayüz pürüzsüzleştirilmiştir.

Silah Döndürme (Spin) ve Silah Değiştirme Rotasyon Bug'ı: Oyunda estetik amaçlı eklenen silah döndürme (R tuşu) animasyonu oynatılırken, oyuncu animasyon bitmeden hızlıca başka bir silaha geçtiğinde, yeni geçen silah eski silahın döndürülme açısında (yamuk bir rotasyonda) takılı kalıyordu ve vuruş açısı görsel olarak sapıtıyordu.
Çözüm olarak; Silah değiştirme scriptinin (WeaponSwitcher veya ilgili kod grubu) içine, her silah geçişi tetiklendiğinde çalışan küçük bir sıfırlama kodu eklendi. Yeni silah aktif edilmeden hemen önce, kuşanılacak olan silah prefabının localRotation değeri Quaternion.identity (yani 0, 0, 0 başlangıç açısı) olarak zorla sıfırlandı. Böylece animasyon yarıda kesilse bile yeni gelen silahın her zaman kameranın önünde tam olarak düzgün açıyla doğması sağlanarak bu görsel bug tamamen çözüldü.

Bahrican Sezer
Öğrenci No: 2502020011
