using Tawreed.Domain.Entities;

namespace Tawreed.Infrastructure.Data;

public static class SeedConstants
{
    // Units
    public static readonly Guid UnitKg = new("7040ee9a-21d6-48ed-b596-1dd9603e53ff");
    public static readonly Guid UnitG = new("14eb1c86-473b-42f3-9b8b-512e534ecb16");
    public static readonly Guid UnitL = new("1c3b36d9-216d-4480-929b-71e611773892");
    public static readonly Guid UnitMl = new("c92ed86f-1de1-4844-8937-1d4da75a3e45");
    public static readonly Guid UnitPiece = new("25275027-d132-4536-a6ba-8f18d71bcec3");
    public static readonly Guid UnitPacket = new("9729ee93-9e25-44d9-b137-48921d676c08");
    public static readonly Guid UnitCarton = new("d8091c00-a214-4c3b-8b9e-28259a79feed");
    public static readonly Guid UnitBottle = new("1bb8edb8-e7f0-49d4-ac09-95c064de2d93");
    public static readonly Guid UnitPack = new("6c7ff7da-b24f-4458-ab32-8a72290bece4");

    // Regions (deterministic GUIDs based on egypt-cities-main dataset)
    // Governorates (prefix 00000000-...)
    public static readonly Guid RegionCairo = new("00000000-0000-0000-0000-000000000004");
    public static readonly Guid RegionAlexandria = new("00000000-0000-0000-0000-000000000005");
    public static readonly Guid RegionGiza = new("00000000-0000-0000-0000-000000000017");
    public static readonly Guid RegionSharqia = new("00000000-0000-0000-0000-000000000010");
    public static readonly Guid RegionDakahlia = new("00000000-0000-0000-0000-000000000009");
    public static readonly Guid RegionBeheira = new("00000000-0000-0000-0000-000000000015");
    public static readonly Guid RegionGharbia = new("00000000-0000-0000-0000-000000000013");
    public static readonly Guid RegionMonufia = new("00000000-0000-0000-0000-000000000014");
    public static readonly Guid RegionQalyubia = new("00000000-0000-0000-0000-000000000011");
    public static readonly Guid RegionPortSaid = new("00000000-0000-0000-0000-000000000006");
    public static readonly Guid RegionSuez = new("00000000-0000-0000-0000-000000000007");
    public static readonly Guid RegionDamietta = new("00000000-0000-0000-0000-000000000008");
    public static readonly Guid RegionFayoum = new("00000000-0000-0000-0000-000000000019");
    public static readonly Guid RegionMinya = new("00000000-0000-0000-0000-000000000020");
    public static readonly Guid RegionAssiut = new("00000000-0000-0000-0000-000000000021");
    public static readonly Guid RegionSohag = new("00000000-0000-0000-0000-000000000022");
    public static readonly Guid RegionLuxor = new("00000000-0000-0000-0000-000000000025");
    public static readonly Guid RegionAswan = new("00000000-0000-0000-0000-000000000024");
    public static readonly Guid RegionIsmailia = new("00000000-0000-0000-0000-000000000016");
    // Centers / Cities (prefix 00000001-...)
    public static readonly Guid RegionNasrCity = new("00000001-0000-0000-0000-000000000029");
    public static readonly Guid RegionMaadi = new("00000001-0000-0000-0000-000000000007");
    public static readonly Guid RegionMohandeseen = new("00000001-0000-0000-0000-000000000209");
    public static readonly Guid RegionOctober6 = new("00000001-0000-0000-0000-000000000214");
    public static readonly Guid RegionSmouha = new("00000001-0000-0000-0000-000000000047");
    // Villages (prefix 00000002-...)
    public static readonly Guid RegionAgamy = new("00000002-0000-0000-0000-000000000435");

    // Categories
    public static readonly Guid CatDairy = new("aaff3e05-39b3-4562-ab68-478dac9fda2b");
    public static readonly Guid CatBeverages = new("49a6ee6e-c20e-4e18-aafc-56dacdb93fa9");
    public static readonly Guid CatWater = new("5752078d-9de9-4021-8a21-e42d495be44a");
    public static readonly Guid CatSnacks = new("26478bbc-1417-43c6-83ff-5cb0b39928f6");
    public static readonly Guid CatMeat = new("634f8595-0ab8-4681-a80c-efd63b59082f");
    public static readonly Guid CatProduce = new("05939dd5-afc3-4e6d-9160-d62714a093ba");
    public static readonly Guid CatOil = new("6ab2adc7-0583-4b27-8f2f-19c074034789");
    public static readonly Guid CatSweets = new("da63afae-3afb-4a14-8832-a7cc225448eb");
    public static readonly Guid CatGrains = new("e76081dd-0555-406b-8d7e-33135dd780ec");
    public static readonly Guid CatCleaning = new("1190147e-e455-47be-a8f2-78b428631e7f");
    public static readonly Guid CatMilk = new("628e3d75-e014-476c-a816-807393f2e09f");
    public static readonly Guid CatYogurt = new("73c7a94c-92c7-4b2b-96c9-b94fa1cc4fd0");
    public static readonly Guid CatCheese = new("f4513a34-620f-4e7c-b480-30364a6478ec");
    public static readonly Guid CatJuice = new("46abead3-2a7c-4124-b917-9aa1d6895147");
    public static readonly Guid CatTeaCoffee = new("71cf5b52-e672-474c-b4c7-7701d3a1ebc7");
    public static readonly Guid CatChips = new("19abea6a-4522-43aa-96ea-456c7c7aa53b");
    public static readonly Guid CatBiscuit = new("95f91595-d5e8-4361-a633-ec27318c7393");
    public static readonly Guid CatPoultry = new("945de5f2-4f10-4db4-afec-ee077ddd16a4");
    public static readonly Guid CatFrozen = new("af41250e-6ec7-4ebe-931f-34d053989073");
    public static readonly Guid CatRice = new("d293679a-e545-4009-9c37-eb6c14db24a0");
    public static readonly Guid CatPasta = new("0fb143dd-5940-4dff-97c1-5377e9bbd128");

    // Users
    public static readonly Guid UserAdmin = new("16459628-b0fa-4465-a194-4febc611081d");
    public static readonly Guid UserBuyer1 = new("3ecef09b-9609-4524-97d3-b7eddd08d0e8");
    public static readonly Guid UserBuyer2 = new("255cb26a-f00b-4ff0-813b-56d0204c64d6");
    public static readonly Guid UserBuyer3 = new("4686a8ed-0c74-4648-bd5b-49d7d166679d");
    public static readonly Guid UserBuyer4 = new("86d86b94-fc93-4e9a-a37d-90f5d89a740c");
    public static readonly Guid UserBuyer5 = new("e2d20e2b-aff0-4248-b50c-e5460a248d3a");
    public static readonly Guid UserBuyer6 = new("fc11d91a-3dd4-4f67-87c1-acf40b944d65");
    public static readonly Guid UserBuyer7 = new("69d10245-5505-4853-a8cc-01217ba4f08f");
    public static readonly Guid UserBuyer8 = new("acd25e71-da22-41c7-824e-5a9fd154333a");
    public static readonly Guid UserBuyer9 = new("fa77e489-6f2e-4070-8edb-decc8851585c");
    public static readonly Guid UserBuyer10 = new("9bfa4e4e-e5ce-4d68-bee3-808e0cd3a791");
    public static readonly Guid UserSupplier1 = new("cde4bb82-3c7f-4f20-b0b1-dcc6a20b3c26");
    public static readonly Guid UserSupplier2 = new("e424e914-7c65-4596-b5ee-2ea234643946");
    public static readonly Guid UserSupplier3 = new("fa057c2b-5927-4f9d-ad92-fef953feed8d");
    public static readonly Guid UserSupplier4 = new("e6ea6265-20b4-4c39-a9ed-a77e9a1ebd78");
    public static readonly Guid UserSupplier5 = new("3c9e0478-d108-4fcd-9431-fbea4be5dd54");
    public static readonly Guid UserSupplier6 = new("eee99f22-24f6-4e6b-b9dc-05b7b99cadd0");
    public static readonly Guid UserSupplier7 = new("6f2336cd-5d33-41dc-8ab9-874ab779fd03");
    public static readonly Guid UserSupplier8 = new("f803a6f5-d484-423a-b48a-3b51fa7f3ad7");
    public static readonly Guid UserSupplier9 = new("ff8d954d-149f-4c79-a2f2-df6ab74bfc4b");
    public static readonly Guid UserSupplier10 = new("0d7bbea3-6470-4ed5-b473-37cd22f45b95");

    // Suppliers (Id PK values — distinct from UserId)
    public static readonly Guid Supplier1 = new("d6a1a124-f64a-4b4f-b7cd-03642969e000");
    public static readonly Guid Supplier2 = new("40c42c46-3774-4ee0-ad5f-d6b16c8c0f6f");
    public static readonly Guid Supplier3 = new("5374892c-cf76-4192-ad6a-f7142bcb1842");
    public static readonly Guid Supplier4 = new("f6120f7e-6c9a-4c61-a0d0-33cf4e96a791");
    public static readonly Guid Supplier5 = new("d3d792d0-2e84-4415-8345-90cf3eef943f");
    public static readonly Guid Supplier6 = new("d967d2d6-a707-430f-b404-9b9b5858086e");
    public static readonly Guid Supplier7 = new("59925002-c1a0-4489-b279-943f1269819e");
    public static readonly Guid Supplier8 = new("590d8027-2ace-4e51-b627-9a10c5fcfce1");
    public static readonly Guid Supplier9 = new("5cd98e0e-585f-4a0a-a529-ac384684ad1b");
    public static readonly Guid Supplier10 = new("fcf3bdbc-9e25-48b2-96b0-a00a755cef06");

    // Buyer PKs (Id PK values — distinct from UserId)
    public static readonly Guid Buyer1 = new("5bc19380-dbcc-4d71-b7cd-261fd03d1797");
    public static readonly Guid Buyer2 = new("34b72b5a-a8e4-46fe-bda4-cba1dc8fc5a0");
    public static readonly Guid Buyer3 = new("98147dda-6c82-4ffa-bb83-2dd64c03d9d2");
    public static readonly Guid Buyer4 = new("247babb0-a7cd-4af3-a292-b5e313ce85d0");
    public static readonly Guid Buyer5 = new("91b476cf-b4e6-4142-a57b-e9889ab16957");
    public static readonly Guid Buyer6 = new("da16cd54-e3dc-4b3e-b167-069060b2780b");
    public static readonly Guid Buyer7 = new("3e701ef7-758c-4631-b371-9de27d609984");
    public static readonly Guid Buyer8 = new("40b88e70-d397-4afb-a2ca-13be42ac8f8c");
    public static readonly Guid Buyer9 = new("174f74c9-72fc-4291-b9ce-330f8753a73c");
    public static readonly Guid Buyer10 = new("e0756572-63d6-486d-af94-cfc5eb46c508");

    // SupplierCategories
    public static readonly Guid SuppCat1 = new("94c882a8-a441-4e20-b419-fd50fd7158a8");
    public static readonly Guid SuppCat2 = new("f34fd06a-c07e-4562-8b78-846e9998cadb");
    public static readonly Guid SuppCat3 = new("b28f6b24-2e97-4323-8d22-19daf14a6ae8");
    public static readonly Guid SuppCat4 = new("bafba1f3-d28c-4116-bea8-608b9384a3b5");
    public static readonly Guid SuppCat5 = new("f6fe8f28-0bad-448b-a1c1-f184e4c902dc");
    public static readonly Guid SuppCat6 = new("97b1d44c-8258-4c19-a8f2-ae686aa2df93");
    public static readonly Guid SuppCat7 = new("d801fa5d-b2df-46b5-9fca-1aa44d1e59d8");
    public static readonly Guid SuppCat8 = new("b26c42cc-61d2-46ee-b941-3a2f22081020");
    public static readonly Guid SuppCat9 = new("5dae8b11-662f-4881-9a8f-45da43399ec6");
    public static readonly Guid SuppCat10 = new("6ef2d290-dd01-4da2-822f-61fe0f7cbaea");
    public static readonly Guid SuppCat11 = new("05954fe1-8a43-43e9-bdcd-95c295c8a56a");
    public static readonly Guid SuppCat12 = new("20c30c26-bc16-4493-918b-ee36802b2bf2");
    public static readonly Guid SuppCat13 = new("0bf0537c-2715-4dcf-ab1c-d93f0be7f6c2");
    public static readonly Guid SuppCat14 = new("cd100001-b0cc-4c88-9818-12ed93c29fa1");
    public static readonly Guid SuppCat15 = new("c7462776-17ea-4e13-b44d-3320c5e5c4fc");
    public static readonly Guid SuppCat16 = new("140c7e85-fa76-44c3-b412-0ee86bcded0e");
    public static readonly Guid SuppCat17 = new("cdc573b1-7e53-49c8-983e-90bd0db0c754");
    public static readonly Guid SuppCat18 = new("048759d0-4d91-4112-a876-21ca30f4e76d");
    public static readonly Guid SuppCat19 = new("de8b1de9-2020-4263-8b8a-2bfa286f2c54");
    public static readonly Guid SuppCat20 = new("edbff281-32a6-4e9e-abbe-e15b5c765b76");
    public static readonly Guid SuppCat21 = new("69c72422-5af6-434c-a5d3-7f8e3f56012f");
    public static readonly Guid SuppCat22 = new("d1da64f4-34fa-4810-af09-383658a91d7f");
    public static readonly Guid SuppCat23 = new("a75e1c7b-a6dc-4e74-9673-f31cc4f44e00");
    public static readonly Guid SuppCat24 = new("2bb7f2ce-c60a-4b49-9674-7e13213cf4e0");
    public static readonly Guid SuppCat25 = new("1a83e0f7-3374-420e-acb4-1534758bf841");
    public static readonly Guid SuppCat26 = new("1956aa1c-4ec5-43b4-95e7-b8add494b51b");

    // Products
    public static readonly Guid Product1 = new("ace045a1-907c-4d76-939f-fd03d0e84a11");
    public static readonly Guid Product2 = new("45f06ed3-d5fa-4343-9d52-a87ecfa5bae1");
    public static readonly Guid Product3 = new("e5aec121-254e-42c9-a676-80453debbf89");
    public static readonly Guid Product4 = new("cf09c9ba-8c7e-4d32-a9e4-f7f4c3ec2746");
    public static readonly Guid Product5 = new("4c9fa6f8-d251-466a-814f-1032d967e967");
    public static readonly Guid Product6 = new("f80382e9-94b5-4ca0-b356-232398badbdb");
    public static readonly Guid Product7 = new("75a1ba53-6f03-4e19-a309-9a48c942a457");
    public static readonly Guid Product8 = new("888aae79-f1d4-4e2a-af83-e62792d8eff4");
    public static readonly Guid Product9 = new("65f927b8-ded7-4ed6-9448-b0f28c48fb1f");
    public static readonly Guid Product10 = new("085218ae-4f81-4815-8428-33de12e3bef5");
    public static readonly Guid Product11 = new("dc0b12cb-7686-4d36-88ac-e721966fb65f");
    public static readonly Guid Product12 = new("945caeac-565f-4260-b379-485b70aa0d7e");
    public static readonly Guid Product13 = new("366a5f46-4aa5-4336-b419-456d64fca6d0");
    public static readonly Guid Product14 = new("0da04f7e-a779-45dd-98e3-0558971e6dd7");
    public static readonly Guid Product15 = new("76f4fd44-f497-4e75-96c9-b21f37d47e41");
    public static readonly Guid Product16 = new("f7b1cd26-efa7-4859-8ecf-95facdfcc4d4");
    public static readonly Guid Product17 = new("ec7275f7-13c2-4305-849f-bd71660046d3");
    public static readonly Guid Product18 = new("571639f8-63d7-4715-9e12-34c2c35a4098");
    public static readonly Guid Product19 = new("b9654c58-a387-4fc1-aaf9-7dae7839638c");
    public static readonly Guid Product20 = new("e1e52a0f-1f22-4071-8562-e8df1c4e515e");
    public static readonly Guid Product21 = new("b2bd0cdb-5ccb-4816-bb1d-795daf2d248f");
    public static readonly Guid Product22 = new("ed9a09e8-3842-4bc8-bbd6-935f58b1b97a");
    public static readonly Guid Product23 = new("d2fd9e66-456b-4773-851e-86c5a7bdd77c");
    public static readonly Guid Product24 = new("b7138f5f-4e7c-47e5-8f8e-7cd3108fb1a1");
    public static readonly Guid Product25 = new("5ee00cf1-c0ad-43d7-a7fb-14da2f1c1034");
    public static readonly Guid Product26 = new("0a869846-885f-4383-bc2b-01a987b1d719");
    public static readonly Guid Product27 = new("d3a8ec1a-b14a-447c-938f-195210354111");
    public static readonly Guid Product28 = new("5f815f2c-98fd-4070-a3b2-e48b8e3783b4");
    public static readonly Guid Product29 = new("c56d0e5c-19c1-4490-9e96-b541b16c7b67");
    public static readonly Guid Product30 = new("0e907b2b-b45d-4bad-833b-f29edab44fec");
    public static readonly Guid Product31 = new("17f2af19-6ca1-402a-93f5-a08193696c07");
    public static readonly Guid Product32 = new("a06d4bca-a8ec-4f2a-a19c-9978964fd086");
    public static readonly Guid Product33 = new("c55cedf9-be8b-4aa6-ad96-1a2dd7c2415b");
    public static readonly Guid Product34 = new("07070f15-a49e-49c7-a981-61fa8264550b");
    public static readonly Guid Product35 = new("bf01f346-7342-47e2-a3be-3ccadcb89f2a");
    public static readonly Guid Product36 = new("bc5cd6ef-68f9-483c-8969-5df770449d62");
    public static readonly Guid Product37 = new("b9a84540-03bb-41f7-a039-3065484f7630");
    public static readonly Guid Product38 = new("d344ee05-027f-4d00-9f53-13fa205a7286");
    public static readonly Guid Product39 = new("93999360-d133-42f3-a020-b3cbfb7d407a");
    public static readonly Guid Product40 = new("8b30e673-a339-49e9-8a73-d282b0339489");
    public static readonly Guid Product41 = new("005f9f49-e511-4130-8d38-9bfe78b50cd1");
    public static readonly Guid Product42 = new("5ef8ef3b-6887-4a94-9241-2f61540b1383");
    public static readonly Guid Product43 = new("c8b5ff65-f63f-4ba0-85ff-206b55a0cd70");
    public static readonly Guid Product44 = new("76e31cfe-371b-4241-af12-988ba88bad02");
    public static readonly Guid Product45 = new("08561e57-658c-43f5-ada9-6ef334b0db4a");

    // SupplierProducts
    public static readonly Guid SP1 = new("498f8b38-c046-4f39-aa26-5dc9fc71d1e6");
    public static readonly Guid SP2 = new("386e6517-a6f4-439c-ac08-66567891b551");
    public static readonly Guid SP3 = new("bbed8691-c5fb-4b5a-b217-41e8567e04f6");
    public static readonly Guid SP4 = new("9b19fe06-9646-47d7-8980-9fe43eefccfb");
    public static readonly Guid SP5 = new("94b24c50-66ac-4f1e-9f7b-4e5eacc1b3ef");
    public static readonly Guid SP6 = new("e5de87e8-1cc1-457e-83b6-19519e3d316e");
    public static readonly Guid SP7 = new("32523241-9167-4bbf-9c27-e80de2ccb234");
    public static readonly Guid SP8 = new("c407386d-504a-4982-a4ca-ea4e295f71a7");
    public static readonly Guid SP9 = new("05fed1f7-d897-47d7-a995-060d1fc318c0");
    public static readonly Guid SP10 = new("44342415-e294-4447-87ad-ec6b8cb4a27d");
    public static readonly Guid SP11 = new("23c79c36-0d06-4f38-82ba-995bea9f1581");
    public static readonly Guid SP12 = new("4be5a265-661c-424d-81da-d2ccaf62b44f");
    public static readonly Guid SP13 = new("014f8bd3-fad0-4574-b4ac-2b03a068145d");
    public static readonly Guid SP14 = new("07344751-5378-49e1-ab53-b3ce3c9c858c");
    public static readonly Guid SP15 = new("ae00c922-43bd-4f70-a14b-ed4ced48ee6f");
    public static readonly Guid SP16 = new("8852c760-0bb7-41d0-ab60-c40e8952e00b");
    public static readonly Guid SP17 = new("5c0db4cc-df78-4239-8171-d58eea759596");
    public static readonly Guid SP18 = new("a98ddd3d-d8d3-4970-b93b-35ec658949d0");
    public static readonly Guid SP19 = new("5bad19dd-c964-4173-a0c8-cc2d998ae983");
    public static readonly Guid SP20 = new("1764a349-fe73-4f5d-b6c6-d7129c6cd551");
    public static readonly Guid SP21 = new("d7ce6aa9-2370-4500-b253-e794452a25a6");
    public static readonly Guid SP22 = new("ec7b7c11-07b7-4111-8a03-d77939dfa35f");
    public static readonly Guid SP23 = new("7be8b7de-cb1c-4b4e-a70b-1f10c46a097d");
    public static readonly Guid SP24 = new("f6ca6a94-e901-4cd8-8f00-1e9230a5a07d");
    public static readonly Guid SP25 = new("e517ce37-362b-408e-b9e0-2d578a15582b");
    public static readonly Guid SP26 = new("1129c19f-243e-4178-99d3-8353a02f2f59");
    public static readonly Guid SP27 = new("cad5a936-cab8-496f-a91c-29d9bbc50ea3");
    public static readonly Guid SP28 = new("33803c7c-526d-4a33-aac5-7379210655c2");
    public static readonly Guid SP29 = new("886c4236-be97-44c5-81b3-73a5afe9875a");
    public static readonly Guid SP30 = new("5c576393-2948-41c0-8ac2-5df76b4c6883");
    public static readonly Guid SP31 = new("01ce8c36-26c7-4907-800c-0e9a8f4986c1");
    public static readonly Guid SP32 = new("a49f3e82-2ecc-4207-80e7-4143fe6318f9");
    public static readonly Guid SP33 = new("6b00efe4-3694-474f-af8f-1689d6ef35f7");
    public static readonly Guid SP34 = new("075fc295-26f7-42ae-96f7-c051b69a0cf8");
    public static readonly Guid SP35 = new("1594b968-75c2-427c-b655-4c277c8e7dbe");
    public static readonly Guid SP36 = new("b2c1eda7-f0a3-4244-ad78-8a847def6c8b");
    public static readonly Guid SP37 = new("8d130741-a514-4177-a257-5986981d6965");
    public static readonly Guid SP38 = new("de27b398-53e5-4e92-8d11-15b638d04707");
    public static readonly Guid SP39 = new("999a55e7-cd87-4f5e-9366-d1e3174f9a22");
    public static readonly Guid SP40 = new("6a45271a-25be-4c0f-9640-c6cfe6e64ea6");
    public static readonly Guid SP41 = new("c48bf023-5cc5-4472-9c83-c98a4b942ca8");
    public static readonly Guid SP42 = new("08e711cd-c5e1-448f-9a8d-5ff00a6a5e1b");
    public static readonly Guid SP43 = new("17f74e34-54ce-44b5-9396-17b3848912ba");
    public static readonly Guid SP44 = new("3be5e8d0-00c6-4f4d-bf5d-420c9f546c63");
    public static readonly Guid SP45 = new("6273fc47-46eb-46da-8645-212d8522dfb9");

    // PricingTiers
    public static readonly Guid PT1 = new("858f0931-84fe-4479-9fc7-fc45690987ad");
    public static readonly Guid PT2 = new("4b49a621-5de1-4336-95d9-bb0b6f2e6d1c");
    public static readonly Guid PT3 = new("0a2e6a4e-55ab-4f81-9b69-4bea1a0d16b0");
    public static readonly Guid PT4 = new("886c011f-0a08-4f34-9ab3-c85ef4a7edba");
    public static readonly Guid PT5 = new("adcabe34-b392-4fb3-b039-23342fd65a2f");
    public static readonly Guid PT6 = new("a8c0ca49-fa2e-4967-aa59-ae8e91e866a3");
    public static readonly Guid PT7 = new("cd3eea3b-83e8-433b-b075-61aba9faf5ed");
    public static readonly Guid PT8 = new("0ddc5a6b-55f8-4c9e-9b41-c89c30810b44");
    public static readonly Guid PT9 = new("9a1112a1-3ee9-434e-9261-85e262bec3d3");
    public static readonly Guid PT10 = new("c6c06ec8-df60-4f23-a648-141afec36705");
    public static readonly Guid PT11 = new("9b99600e-60f5-4ad3-9389-f281c1e1e2aa");
    public static readonly Guid PT12 = new("32077427-6284-415b-9183-65e2f4d7692d");
    public static readonly Guid PT13 = new("02449ea4-3316-472f-ac1e-f96aa117d153");
    public static readonly Guid PT14 = new("08dfb446-5f14-408b-a033-0c737eab019b");
    public static readonly Guid PT15 = new("930efacf-2f69-473d-b079-74c1f9bfef74");
    public static readonly Guid PT16 = new("8a85b359-7465-42bf-b633-6eeed27fe785");
    public static readonly Guid PT17 = new("8565cb70-54f0-4acc-8b01-d2bdda8a7563");
    public static readonly Guid PT18 = new("11ea3838-6d0c-45ec-8302-8ac3751d4d39");
    public static readonly Guid PT19 = new("b10ff8f3-a329-41dd-957b-0a942924bb62");
    public static readonly Guid PT20 = new("79c1ad25-9e1e-43c3-9c18-a676c858b666");
    public static readonly Guid PT21 = new("bc9c8100-735f-4748-a49f-632d0b449163");
    public static readonly Guid PT22 = new("44d45475-e222-4e0d-8f01-37ab12e1705d");
    public static readonly Guid PT23 = new("e0636f5b-753a-4d95-9f08-4f2d082093d4");
    public static readonly Guid PT24 = new("7e6228a1-a8f0-4f70-ad53-2b4bf2fefdf0");
    public static readonly Guid PT25 = new("d0a78f38-5300-44c2-8ea7-e501691dffbe");
    public static readonly Guid PT26 = new("15fffea6-22cc-4203-b836-490457291fb6");
    public static readonly Guid PT27 = new("2c2478c3-8ce0-43bf-94c9-6967104d04d7");
    public static readonly Guid PT28 = new("48ab75b3-b560-40c2-b336-20bf920175f9");
    public static readonly Guid PT29 = new("556db15a-e567-4def-992e-03feb153ed3c");
    public static readonly Guid PT30 = new("6adae785-d7fb-4411-965f-da48e4e566ca");
    public static readonly Guid PT31 = new("411a6d6e-2503-483b-8bef-f5f2b7d3d6d0");
    public static readonly Guid PT32 = new("e8402ecc-fc68-4089-94cf-22be8ac01ddd");
    public static readonly Guid PT33 = new("37b82e1f-c206-4ac7-a9a2-96302d0065e6");
    public static readonly Guid PT34 = new("64abdaf8-55fc-4a98-89f3-fedc37bb5de0");
    public static readonly Guid PT35 = new("f5884733-4385-4b9f-b3e7-18f5db141c57");
    public static readonly Guid PT36 = new("0be34b14-56b5-4a7d-860e-ea6fffaf21e5");
    public static readonly Guid PT37 = new("095c96d0-a973-4e79-b564-8d603f1ef45f");
    public static readonly Guid PT38 = new("007a19f0-57b1-4c9b-b9e2-73bd856725d4");
    public static readonly Guid PT39 = new("0892df71-41fc-4f54-84cf-7f04082f7e4e");
    public static readonly Guid PT40 = new("45c3cd9f-a9be-4044-911b-1fe1b00b9400");
    public static readonly Guid PT41 = new("c6d305f3-c8f3-421d-a881-cd3b543bb3c2");
    public static readonly Guid PT42 = new("a97ed737-1753-4771-8b71-05d735048a68");
    public static readonly Guid PT43 = new("9e6e6ff7-4e13-4abd-a31e-20a41eeb70cb");
    public static readonly Guid PT44 = new("53b61be3-24ed-4331-b332-f30921195e9b");
    public static readonly Guid PT45 = new("8dfcb98d-a74d-4485-b44a-8b28bf6f81ba");
    public static readonly Guid PT46 = new("3f3bd3a3-e768-4887-9cf6-a9d99d3d61c6");
    public static readonly Guid PT47 = new("bf53a357-e11e-4884-a345-fceab31e0a98");
    public static readonly Guid PT48 = new("b1877f76-9157-499d-8566-d1345365a6d8");
    public static readonly Guid PT49 = new("2770d08d-0976-4b97-91bf-b7e83f288e28");
    public static readonly Guid PT50 = new("dd9fd3cf-4c3f-46ea-91b5-d18d5a679675");
    public static readonly Guid PT51 = new("9a2cd707-d165-4a13-9252-b0146cdbf2c1");
    public static readonly Guid PT52 = new("99ad6c18-1d13-4911-bb8e-6aae460ec39f");
    public static readonly Guid PT53 = new("028936f9-45bf-4444-89ff-1acf06ce8ab6");
    public static readonly Guid PT54 = new("2c39707e-900d-4385-a8cd-4a0d6dcc3491");
    public static readonly Guid PT55 = new("eba29da5-d60e-48d8-9924-ad9bdf66eb6e");
    public static readonly Guid PT56 = new("55a88fe1-b48f-4def-9129-db78d252a0c6");
    public static readonly Guid PT57 = new("5d693860-f903-4d02-b537-df46a044b0a2");
    public static readonly Guid PT58 = new("e365ef01-e213-41ac-a446-a71bad1afa3d");
    public static readonly Guid PT59 = new("eed373d1-6266-41a6-9eba-7469ddf6b806");
    public static readonly Guid PT60 = new("35d25e98-06eb-47b1-892d-ad8fa585aa0a");
    public static readonly Guid PT61 = new("c3ec92e6-dbea-4aa4-a32b-f334721f438c");
    public static readonly Guid PT62 = new("7820b3b4-d9a4-4f97-b7ac-2a3b835e0dd6");
    public static readonly Guid PT63 = new("0ddd667b-0561-41d7-aa91-24e479409c6a");
    public static readonly Guid PT64 = new("f1ed2760-dc4d-4cd4-a9e7-e75f68cade69");
    public static readonly Guid PT65 = new("399894f3-de72-4a30-8581-876765fd5d18");
    public static readonly Guid PT66 = new("612386da-9c83-40b6-af61-b98014f7a47a");
    public static readonly Guid PT67 = new("bfb2dfe0-638b-4bb6-946e-1a60b8f61a4f");
    public static readonly Guid PT68 = new("65f4b91b-196a-47bc-8cf0-49351f97a0cc");
    public static readonly Guid PT69 = new("990d8fb2-b9db-4c51-94ef-22b467f611c8");
    public static readonly Guid PT70 = new("d3c47146-3f88-4f41-b8ec-938f1f7441da");
    public static readonly Guid PT71 = new("6d7de962-e04f-4729-8e20-ab55f841009f");
    public static readonly Guid PT72 = new("ee855bcc-7c24-4964-8f25-f5ff11068e3f");
    public static readonly Guid PT73 = new("dd3ebd51-90bb-4114-a018-b3fcac1b5193");
    public static readonly Guid PT74 = new("410fb2eb-b1c9-48e5-a5ae-f21161efe54d");
    public static readonly Guid PT75 = new("aedb6c22-54e6-4a83-9f6c-6a769bac84d7");
    public static readonly Guid PT76 = new("782bb7f9-af73-4ca2-8209-5c567a23509a");
    public static readonly Guid PT77 = new("30a98c92-5b08-4af6-a640-57aacee9be0e");
    public static readonly Guid PT78 = new("5bceeb7f-7f00-4e87-bd71-d306d015b29c");
    public static readonly Guid PT79 = new("ba7eebf4-bb15-4a2c-820d-47601734b454");
    public static readonly Guid PT80 = new("4f8a05fe-32f3-4983-99a8-811c467142d8");
    public static readonly Guid PT81 = new("0a8f2a83-5491-4294-b414-a41a663911fc");
    public static readonly Guid PT82 = new("714518cb-49fd-4ce3-b9e0-9757fa279121");
    public static readonly Guid PT83 = new("ec7edb7b-526c-4dc9-a71d-2d8acd44b6ea");
    public static readonly Guid PT84 = new("bf2ca5e0-1005-4ce2-adf1-abbe3c44b4e1");
    public static readonly Guid PT85 = new("3d17967a-72c7-4016-a707-5a2185944aed");
    public static readonly Guid PT86 = new("7eeaed68-0643-4e15-883e-42f1fafcd327");
    public static readonly Guid PT87 = new("b92594f6-8989-403d-9d25-bf4babfe9a67");
    public static readonly Guid PT88 = new("f015abb9-5f34-4bd8-898b-aab9305c094f");
    public static readonly Guid PT89 = new("b937a108-40b4-4bf8-a556-5fe9b7a1bed9");
    public static readonly Guid PT90 = new("2da4f167-1446-418b-85f3-72b864119e31");

    // ProductImages
    public static readonly Guid ProdImg1 = new("45c8427d-ffa3-434b-894d-5a4189f4773b");
    public static readonly Guid ProdImg2 = new("6e877fb2-8900-490c-a637-18cbbdd29447");
    public static readonly Guid ProdImg3 = new("f37c77ad-43e0-41f9-9393-7767b4d2f421");
    public static readonly Guid ProdImg4 = new("e735d107-2614-4895-aaa2-42d19b288360");
    public static readonly Guid ProdImg5 = new("e8b96e78-fce4-4bf1-8bdc-81ee86b7f75b");
    public static readonly Guid ProdImg6 = new("24380d5e-282c-452f-9b55-51c46144935b");
    public static readonly Guid ProdImg7 = new("e64aba3a-cc92-4699-9459-898d0da1ec36");
    public static readonly Guid ProdImg8 = new("0c07219d-fb6b-4c18-ba74-7790e8ab0fc2");
    public static readonly Guid ProdImg9 = new("3d9b3947-2c9f-4f0e-80ed-508a826fcd32");
    public static readonly Guid ProdImg10 = new("b212e932-1f7a-4a2e-b558-196aafc3eb3a");
    public static readonly Guid ProdImg11 = new("fd2d40db-ffcc-4c3a-a48b-c4d8445a8d5c");
    public static readonly Guid ProdImg12 = new("934eb075-f270-4465-819d-b012a9885212");
    public static readonly Guid ProdImg13 = new("966b183e-65ed-414a-a5d3-ee8b7ef537b9");
    public static readonly Guid ProdImg14 = new("bcfba866-a093-45aa-b74d-1a21f763683a");
    public static readonly Guid ProdImg15 = new("63c26cd6-ac34-4305-b068-f314331a597e");
    public static readonly Guid ProdImg16 = new("a56bc6e5-b6ac-4aac-a027-2d1e06379794");
    public static readonly Guid ProdImg17 = new("4c59d0e3-4328-4efa-9832-ecf115843d07");
    public static readonly Guid ProdImg18 = new("71a34886-3fff-42d2-b348-fb5b26cfb722");
    public static readonly Guid ProdImg19 = new("bc3bd43f-7ad7-4159-9a7c-2006e66c435d");
    public static readonly Guid ProdImg20 = new("a79dd495-be8c-4263-893e-0f17b984ae84");
    public static readonly Guid ProdImg21 = new("2c8ba525-ffff-4a6f-8f1d-58e4ac399ee3");
    public static readonly Guid ProdImg22 = new("e65cd643-6179-4434-b90e-41b52b2b24b8");
    public static readonly Guid ProdImg23 = new("2c8d83b0-3cf5-4d7f-b76c-d8aee6aa55f6");
    public static readonly Guid ProdImg24 = new("c78d49e3-1d7a-4ce5-856f-1fd97739e5f7");
    public static readonly Guid ProdImg25 = new("71b58564-9a5e-4755-b9e6-6df0161d927b");
    public static readonly Guid ProdImg26 = new("293195b7-a6b1-4879-a9d1-35a8f9c87ae1");
    public static readonly Guid ProdImg27 = new("2890a7fa-6421-49e6-8184-5abc097a20bf");
    public static readonly Guid ProdImg28 = new("157df328-5f54-4a2e-a92e-673a2c987d3f");
    public static readonly Guid ProdImg29 = new("70b64942-6490-4ca0-a43a-9fa90caa1acd");
    public static readonly Guid ProdImg30 = new("53f8715e-3fd0-4005-8455-38347037803a");
    public static readonly Guid ProdImg31 = new("8709e23d-1376-4e51-835c-aaeaebab98c2");
    public static readonly Guid ProdImg32 = new("55c2e3e9-dae5-4533-8ebe-0abfdef12dfb");
    public static readonly Guid ProdImg33 = new("d411d3da-79f0-4f68-a622-9c0c7a45ef18");
    public static readonly Guid ProdImg34 = new("6dc55080-88df-462f-8430-1389fe1810c7");
    public static readonly Guid ProdImg35 = new("efe0033e-2f2f-4fd1-95a2-c7e77d41219a");
    public static readonly Guid ProdImg36 = new("d05bef01-746a-4377-a9f1-7a6279ef46ca");
    public static readonly Guid ProdImg37 = new("df9d187b-b4d4-4ca0-a5c0-2a36ce330884");
    public static readonly Guid ProdImg38 = new("99d2a0a6-b9e7-4f9e-862b-385ad120aa77");
    public static readonly Guid ProdImg39 = new("73e3b7bd-39e7-43da-b698-c22cd5fc811e");
    public static readonly Guid ProdImg40 = new("c32bfe74-fc53-4e76-85a6-9c690c5e895e");
    public static readonly Guid ProdImg41 = new("68de5b5b-eee6-4828-93b6-a2be0a412d12");
    public static readonly Guid ProdImg42 = new("97fff2fe-74c0-4615-8e49-df727a1f4dc9");
    public static readonly Guid ProdImg43 = new("3e1045ef-a7c4-4f2d-b702-1b700a891110");
    public static readonly Guid ProdImg44 = new("6017aa89-325b-48e0-88ab-800a64047034");
    public static readonly Guid ProdImg45 = new("5dee5bf1-9382-4e3a-9644-4928b516091b");

    // GroupOrders
    public static readonly Guid Order1 = new("bba56c6a-827e-4394-a10a-b81c6d9dbeaf");
    public static readonly Guid Order2 = new("6b1ac916-146a-44e8-a523-7d69d833f055");
    public static readonly Guid Order3 = new("3d1335a9-a25c-4762-9f39-8046fc475f7a");
    public static readonly Guid Order4 = new("85085d3d-4938-4ba1-8b6a-2ef99f9995b0");
    public static readonly Guid Order5 = new("38e769df-7e03-4d47-82e2-4fe4c7a6d3ae");
    public static readonly Guid Order6 = new("7e0d7a95-b9f5-40d9-beaf-4ac077593cf5");
    public static readonly Guid Order7 = new("382e6a64-ea3a-4dcc-8c05-379aa01a3461");

    // GroupOrderItems
    public static readonly Guid OrderItem1 = new("1e1cfe17-cc86-43ab-b7ff-b27bb1990a21");
    public static readonly Guid OrderItem2 = new("bcb1ab44-59e5-4fd4-8540-ca210f2c2af5");
    public static readonly Guid OrderItem3 = new("7c4cc6ff-87e3-4dc8-8bc8-dec4466f8a30");
    public static readonly Guid OrderItem4 = new("ae7c6cdc-a80b-482e-944b-63a073d4f8f5");
    public static readonly Guid OrderItem5 = new("5481a4c0-9bff-4aaa-a059-bdb3702631fb");
    public static readonly Guid OrderItem6 = new("334c1051-ab0a-41e4-8522-08a717723e57");
    public static readonly Guid OrderItem7 = new("723a6ef4-d0e4-4640-8631-c55ee207ca25");
    public static readonly Guid OrderItem8 = new("09ce0e6a-c14b-474b-8f57-9629c0c76cb4");
    public static readonly Guid OrderItem9 = new("3171eaf2-0921-4eb8-a481-6d20313b59bf");

    // Participants
    public static readonly Guid Participant1 = new("3694114e-406b-4561-b5ed-04ca0aea0f32");
    public static readonly Guid Participant2 = new("7ca5a39b-7d08-41c3-86d4-6e7d22a0d2ce");
    public static readonly Guid Participant3 = new("f0a01ad3-aef3-4d76-8a2f-a32fa14a9faa");
    public static readonly Guid Participant4 = new("779396d9-8886-4c37-b4e8-851597c389cc");

    // ParticipantItems
    public static readonly Guid PartItem1 = new("aa6d806f-c565-4690-b1a7-722a0bfe8ad8");
    public static readonly Guid PartItem2 = new("101d3c7a-9ace-4ae2-b2a3-92e3282d4c37");
    public static readonly Guid PartItem3 = new("ffe232d6-dc93-4e45-87f1-be895a30d67d");
    public static readonly Guid PartItem4 = new("a77e4044-066a-4a50-81d9-3e85c79aa5de");
    public static readonly Guid PartItem5 = new("c3bf1857-e9b4-4576-a981-f548fad845c1");

    // Events
    public static readonly Guid Event1 = new("7d2b6d80-0487-4c6e-9827-843343669ec8");
    public static readonly Guid Event2 = new("67efc413-1d78-4b38-912d-ab4ab2226af9");
    public static readonly Guid Event3 = new("45d6fe32-f540-49a3-a4a5-7055f5954110");
    public static readonly Guid Event4 = new("933038da-3a53-4b02-b51c-c728e507aedd");
    public static readonly Guid Event5 = new("3e42bb9d-22dd-4a16-99c7-b5c481a92baf");
    public static readonly Guid Event6 = new("3a557607-85a5-4c04-9972-aea7f9843ff0");
    public static readonly Guid Event7 = new("2dba9873-f22c-41c0-bd78-156435d30e26");
    public static readonly Guid Event8 = new("d0d7a7a2-9001-4348-83b4-f4618e4774dd");

    // Notifications
    public static readonly Guid Notification1 = new("fb61558d-8b3f-4dac-b4b9-c0ef2dc89b3f");
    public static readonly Guid Notification2 = new("a18e224d-7cee-4365-9f2f-44ad9ebf72f2");
    public static readonly Guid Notification3 = new("35fcc1ab-2bdd-4d25-8028-b63e924f1e5b");
    public static readonly Guid Notification4 = new("80202a6e-7b62-4050-8c9d-7e6099f9d465");
    public static readonly Guid Notification5 = new("837a1e2d-e76a-4d45-bdf4-ffd354870eb3");

    // Delivery Persons
    public static readonly Guid UserDeliveryPerson1 = new("11111111-1111-4111-8111-111111111001");
    public static readonly Guid UserDeliveryPerson2 = new("22222222-2222-4222-8222-222222222001");
    public static readonly Guid DeliveryPersonProfile1 = new("aaaaaaa1-1111-4a1a-8a1a-aaaaaaaaaa01");
    public static readonly Guid DeliveryPersonProfile2 = new("bbbbbbb2-2222-4b2b-8b2b-bbbbbbbbbb02");
}
