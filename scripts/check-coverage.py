#!/usr/bin/env python3
"""Une os relatorios Cobertura (unit + integration) e valida a cobertura minima de linhas.

Uso: python scripts/check-coverage.py <diretorio-coverage> <minimo-percentual>
"""
import sys
import glob
import xml.etree.ElementTree as ET

diretorio = sys.argv[1]
minimo = float(sys.argv[2])

linhas = {}  # (arquivo, numero) -> max hits
for relatorio in glob.glob(f"{diretorio}/**/coverage.cobertura.xml", recursive=True):
    for classe in ET.parse(relatorio).getroot().iter("class"):
        arquivo = classe.get("filename")
        for linha in classe.iter("line"):
            chave = (arquivo, linha.get("number"))
            linhas[chave] = max(linhas.get(chave, 0), int(linha.get("hits")))

if not linhas:
    print("Nenhum relatorio de cobertura encontrado.")
    sys.exit(1)

cobertas = sum(1 for hits in linhas.values() if hits > 0)
percentual = 100 * cobertas / len(linhas)
print(f"Cobertura de linhas: {percentual:.2f}% ({cobertas}/{len(linhas)}) - minimo exigido: {minimo:.0f}%")

if percentual < minimo:
    print("FALHA: cobertura abaixo do minimo.")
    sys.exit(1)
print("OK: cobertura minima atingida.")
