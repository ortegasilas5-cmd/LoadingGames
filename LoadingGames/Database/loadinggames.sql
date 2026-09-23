-- ============================================================
-- BANCO DE DADOS: LOADING GAMES
-- ============================================================

DROP DATABASE IF EXISTS LoadingGames;



CREATE DATABASE LoadingGames
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE LoadingGames;
select * from jogo;
SHOW TABLES;

-- ============================================================
-- 1. USUÁRIO
-- ============================================================

CREATE TABLE usuario (
    id_usuario INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    nome_usuario VARCHAR(50) NOT NULL,
    email VARCHAR(150) NOT NULL,
    senha_hash VARCHAR(255) NOT NULL,
    foto_perfil_url VARCHAR(500),
    status_conta ENUM('Ativo', 'Inativo') NOT NULL DEFAULT 'Ativo',
    tipo_usuario ENUM('Comum', 'Administrador') NOT NULL DEFAULT 'Comum',
    data_cadastro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id_usuario),

    CONSTRAINT uq_usuario_nome_usuario
        UNIQUE (nome_usuario),

    CONSTRAINT uq_usuario_email
        UNIQUE (email)
);


-- ============================================================
-- 2. DESENVOLVEDOR
-- ============================================================

CREATE TABLE desenvolvedor (
    id_desenvolvedor INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(150) NOT NULL,
    descricao TEXT,

    PRIMARY KEY (id_desenvolvedor)
);


-- ============================================================
-- 3. PUBLICADORA
-- ============================================================

CREATE TABLE publicadora (
    id_publicadora INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(150) NOT NULL,
    descricao TEXT,

    PRIMARY KEY (id_publicadora)
);


-- ============================================================
-- 4. GÊNERO
-- ============================================================

CREATE TABLE genero (
    id_genero INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    descricao VARCHAR(255),

    PRIMARY KEY (id_genero),

    CONSTRAINT uq_genero_nome
        UNIQUE (nome)
);


-- ============================================================
-- 5. TAG
-- ============================================================

CREATE TABLE tag (
    id_tag INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,

    PRIMARY KEY (id_tag),

    CONSTRAINT uq_tag_nome
        UNIQUE (nome)
);


-- ============================================================
-- 6. PLATAFORMA
-- ============================================================

CREATE TABLE plataforma (
    id_plataforma INT NOT NULL AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,

    PRIMARY KEY (id_plataforma),

    CONSTRAINT uq_plataforma_nome
        UNIQUE (nome)
);


-- ============================================================
-- 7. JOGO
-- ============================================================

CREATE TABLE jogo (
    id_jogo INT NOT NULL AUTO_INCREMENT,
    id_desenvolvedor INT NOT NULL,
    id_publicadora INT NOT NULL,

    nome VARCHAR(150) NOT NULL,
    descricao TEXT,
    preco DECIMAL(10,2) NOT NULL DEFAULT 0.00,

    imagem_principal_url VARCHAR(500),

    data_lancamento DATE,

    -- 0 = Livre, 10 = 10 anos, 12 = 12 anos,
    -- 14 = 14 anos, 16 = 16 anos, 18 = 18 anos
    classificacao_indicativa TINYINT NOT NULL DEFAULT 0,

    ativo TINYINT(1) NOT NULL DEFAULT 1,

    PRIMARY KEY (id_jogo),

    CONSTRAINT fk_jogo_desenvolvedor
        FOREIGN KEY (id_desenvolvedor)
        REFERENCES desenvolvedor(id_desenvolvedor),

    CONSTRAINT fk_jogo_publicadora
        FOREIGN KEY (id_publicadora)
        REFERENCES publicadora(id_publicadora),

    CONSTRAINT chk_jogo_preco
        CHECK (preco >= 0),

    CONSTRAINT chk_jogo_classificacao
        CHECK (classificacao_indicativa IN (0, 10, 12, 14, 16, 18))
);


-- ============================================================
-- 8. IMAGENS DO JOGO
-- ============================================================

CREATE TABLE imagem_jogo (
    id_imagem INT NOT NULL AUTO_INCREMENT,
    id_jogo INT NOT NULL,
    url_imagem VARCHAR(500) NOT NULL,
    tipo ENUM('Screenshot', 'Galeria') NOT NULL DEFAULT 'Screenshot',

    PRIMARY KEY (id_imagem),

    CONSTRAINT fk_imagem_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
        ON DELETE CASCADE
);


-- ============================================================
-- 9. JOGO x GÊNERO
-- ============================================================

CREATE TABLE jogo_genero (
    id_jogo INT NOT NULL,
    id_genero INT NOT NULL,

    PRIMARY KEY (id_jogo, id_genero),

    CONSTRAINT fk_jogo_genero_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
        ON DELETE CASCADE,

    CONSTRAINT fk_jogo_genero_genero
        FOREIGN KEY (id_genero)
        REFERENCES genero(id_genero)
        ON DELETE CASCADE
);


-- ============================================================
-- 10. JOGO x TAG
-- ============================================================

CREATE TABLE jogo_tag (
    id_jogo INT NOT NULL,
    id_tag INT NOT NULL,

    PRIMARY KEY (id_jogo, id_tag),

    CONSTRAINT fk_jogo_tag_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
        ON DELETE CASCADE,

    CONSTRAINT fk_jogo_tag_tag
        FOREIGN KEY (id_tag)
        REFERENCES tag(id_tag)
        ON DELETE CASCADE
);


-- ============================================================
-- 11. JOGO x PLATAFORMA
-- ============================================================

CREATE TABLE jogo_plataforma (
    id_jogo INT NOT NULL,
    id_plataforma INT NOT NULL,

    PRIMARY KEY (id_jogo, id_plataforma),

    CONSTRAINT fk_jogo_plataforma_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
        ON DELETE CASCADE,

    CONSTRAINT fk_jogo_plataforma_plataforma
        FOREIGN KEY (id_plataforma)
        REFERENCES plataforma(id_plataforma)
        ON DELETE CASCADE
);


-- ============================================================
-- 12. CARRINHO
-- ============================================================

CREATE TABLE carrinho (
    id_carrinho INT NOT NULL AUTO_INCREMENT,
    id_usuario INT NOT NULL,
    data_criacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id_carrinho),

    CONSTRAINT uq_carrinho_usuario
        UNIQUE (id_usuario),

    CONSTRAINT fk_carrinho_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuario(id_usuario)
        ON DELETE CASCADE
);


-- ============================================================
-- 13. ITEM DO CARRINHO
-- ============================================================

CREATE TABLE item_carrinho (
    id_item_carrinho INT NOT NULL AUTO_INCREMENT,
    id_carrinho INT NOT NULL,
    id_jogo INT NOT NULL,
    data_adicao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id_item_carrinho),

    CONSTRAINT uq_item_carrinho_jogo
        UNIQUE (id_carrinho, id_jogo),

    CONSTRAINT fk_item_carrinho_carrinho
        FOREIGN KEY (id_carrinho)
        REFERENCES carrinho(id_carrinho)
        ON DELETE CASCADE,

    CONSTRAINT fk_item_carrinho_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
);


-- ============================================================
-- 14. COMPRA
-- ============================================================

CREATE TABLE compra (
    id_compra INT NOT NULL AUTO_INCREMENT,
    id_usuario INT NOT NULL,

    data_compra DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    valor_total DECIMAL(10,2) NOT NULL DEFAULT 0.00,

    forma_pagamento ENUM(
        'Cartao',
        'Pix',
        'Boleto'
    ) NOT NULL,

    status_compra ENUM(
        'Pendente',
        'Concluida',
        'Cancelada'
    ) NOT NULL DEFAULT 'Pendente',

    PRIMARY KEY (id_compra),

    CONSTRAINT fk_compra_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuario(id_usuario),

    CONSTRAINT chk_compra_valor
        CHECK (valor_total >= 0)
);


-- ============================================================
-- 15. ITEM DA COMPRA
-- ============================================================

CREATE TABLE item_compra (
    id_item_compra INT NOT NULL AUTO_INCREMENT,
    id_compra INT NOT NULL,
    id_jogo INT NOT NULL,

    -- Preço registrado no momento da compra
    preco_unitario DECIMAL(10,2) NOT NULL,

    PRIMARY KEY (id_item_compra),

    CONSTRAINT uq_item_compra_jogo
        UNIQUE (id_compra, id_jogo),

    CONSTRAINT fk_item_compra_compra
        FOREIGN KEY (id_compra)
        REFERENCES compra(id_compra)
        ON DELETE CASCADE,

    CONSTRAINT fk_item_compra_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo),

    CONSTRAINT chk_item_compra_preco
        CHECK (preco_unitario >= 0)
);


-- ============================================================
-- 16. POSSE / BIBLIOTECA
-- ============================================================

CREATE TABLE posse_jogo (
    id_posse INT NOT NULL AUTO_INCREMENT,
    id_usuario INT NOT NULL,
    id_jogo INT NOT NULL,
    id_compra INT NOT NULL,

    data_aquisicao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (id_posse),

    -- Um usuário não pode possuir o mesmo jogo duas vezes
    CONSTRAINT uq_posse_usuario_jogo
        UNIQUE (id_usuario, id_jogo),

    CONSTRAINT fk_posse_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuario(id_usuario)
        ON DELETE CASCADE,

    CONSTRAINT fk_posse_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo),

    CONSTRAINT fk_posse_compra
        FOREIGN KEY (id_compra)
        REFERENCES compra(id_compra)
);


-- ============================================================
-- 17. AVALIAÇÃO
-- ============================================================

CREATE TABLE avaliacao (
    id_avaliacao INT NOT NULL AUTO_INCREMENT,
    id_usuario INT NOT NULL,
    id_jogo INT NOT NULL,

    tipo_avaliacao ENUM('Positiva', 'Negativa') NOT NULL,
    comentario TEXT,

    data_avaliacao DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_atualizacao DATETIME NULL,

    PRIMARY KEY (id_avaliacao),

    -- Um usuário só pode avaliar um determinado jogo uma vez
    CONSTRAINT uq_avaliacao_usuario_jogo
        UNIQUE (id_usuario, id_jogo),

    CONSTRAINT fk_avaliacao_usuario
        FOREIGN KEY (id_usuario)
        REFERENCES usuario(id_usuario)
        ON DELETE CASCADE,

    CONSTRAINT fk_avaliacao_jogo
        FOREIGN KEY (id_jogo)
        REFERENCES jogo(id_jogo)
        ON DELETE CASCADE
);
 select * from usuario;
 
 -- =====================================================
-- DADOS INICIAIS PARA TESTE DO CATÁLOGO
-- =====================================================

-- =====================================================
-- DESENVOLVEDORES
-- =====================================================

INSERT INTO desenvolvedor (nome, descricao) VALUES
('Rockstar North', 'Estúdio responsável por Grand Theft Auto V.'),
('CD Projekt Red', 'Estúdio responsável por Cyberpunk 2077 e The Witcher 3.'),
('Rockstar Studios', 'Estúdios responsáveis por Red Dead Redemption 2.');


-- =====================================================
-- PUBLICADORAS
-- =====================================================

INSERT INTO publicadora (nome, descricao) VALUES
('Rockstar Games', 'Publicadora da série Grand Theft Auto e Red Dead Redemption.'),
('CD Projekt', 'Publicadora de jogos da CD Projekt Red.');


-- =====================================================
-- GÊNEROS
-- =====================================================

INSERT INTO genero (nome, descricao) VALUES
('Ação', 'Jogos focados em ação e combate.'),
('Aventura', 'Jogos focados em exploração e aventura.'),
('RPG', 'Jogos de interpretação e progressão de personagem.'),
('Mundo Aberto', 'Jogos com grandes ambientes abertos para exploração.');


-- =====================================================
-- JOGOS
-- =====================================================

INSERT INTO jogo
(
    id_desenvolvedor,
    id_publicadora,
    nome,
    descricao,
    preco,
    imagem_principal_url,
    data_lancamento,
    classificacao_indicativa,
    ativo
)
VALUES
(
    1,
    1,
    'Grand Theft Auto V',
    'Explore Los Santos em uma experiência de mundo aberto cheia de ação, aventuras e possibilidades.',
    99.90,
    '/images/capagtav.jpg',
    '2013-09-17',
    18,
    1
),
(
    2,
    2,
    'Cyberpunk 2077',
    'Explore Night City em um RPG de ação ambientado em um futuro tecnológico e perigoso.',
    199.90,
    '/images/capacyberpunk.jpg',
    '2020-12-10',
    18,
    1
),
(
    3,
    1,
    'Red Dead Redemption 2',
    'Viva a história de Arthur Morgan e da gangue Van der Linde no fim da era do Velho Oeste.',
    249.90,
    '/images/capareddead.jpg',
    '2018-10-26',
    18,
    1
),
(
    2,
    2,
    'The Witcher 3: Wild Hunt',
    'Assuma o papel de Geralt de Rívia em uma aventura de RPG por um vasto mundo de fantasia.',
    129.90,
    '/images/capawitcher3.jpg',
    '2015-05-19',
    16,
    1
);


-- =====================================================
-- RELAÇÃO JOGO ↔ GÊNERO
-- =====================================================

-- GTA V
INSERT INTO jogo_genero (id_jogo, id_genero) VALUES
(1, 1),
(1, 2),
(1, 4);

-- Cyberpunk 2077
INSERT INTO jogo_genero (id_jogo, id_genero) VALUES
(2, 1),
(2, 3),
(2, 4);

-- Red Dead Redemption 2
INSERT INTO jogo_genero (id_jogo, id_genero) VALUES
(3, 1),
(3, 2),
(3, 4);

-- The Witcher 3
INSERT INTO jogo_genero (id_jogo, id_genero) VALUES
(4, 1),
(4, 2),
(4, 3),
(4, 4);

SELECT * FROM jogo;

SELECT
    j.nome AS jogo,
    g.nome AS genero
FROM jogo j
INNER JOIN jogo_genero jg
    ON j.id_jogo = jg.id_jogo
INNER JOIN genero g
    ON g.id_genero = jg.id_genero
ORDER BY j.id_jogo, g.nome;
-- ============================================================
-- FIM DO BANCO
-- ============================================================