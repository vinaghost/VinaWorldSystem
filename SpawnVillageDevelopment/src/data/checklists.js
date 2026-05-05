import fourPartySimCsv from './sheets/four-party-sim.csv?raw'
import fourPartyFarmCsv from './sheets/four-party-farm.csv?raw'
import threePartySimCsv from './sheets/three-party-sim.csv?raw'
import threePartyFarmCsv from './sheets/three-party-farm.csv?raw'

function parseCsv(csvText) {
  const rows = []
  let row = []
  let cell = ''
  let inQuotes = false

  for (let i = 0; i < csvText.length; i += 1) {
    const char = csvText[i]

    if (char === '"') {
      const nextChar = csvText[i + 1]
      if (inQuotes && nextChar === '"') {
        cell += '"'
        i += 1
      } else {
        inQuotes = !inQuotes
      }
      continue
    }

    if (char === ',' && !inQuotes) {
      row.push(cell)
      cell = ''
      continue
    }

    if ((char === '\n' || char === '\r') && !inQuotes) {
      if (char === '\r' && csvText[i + 1] === '\n') {
        i += 1
      }

      row.push(cell)
      cell = ''

      if (row.some((value) => value !== '')) {
        rows.push(row)
      }
      row = []
      continue
    }

    cell += char
  }

  if (cell.length > 0 || row.length > 0) {
    row.push(cell)
    if (row.some((value) => value !== '')) {
      rows.push(row)
    }
  }

  return rows
}

function mapChecklistRows(csvText, keyPrefix) {
  const parsed = parseCsv(csvText)
  if (parsed.length <= 1) {
    return []
  }

  const records = parsed.slice(1)

  const getAt = (record, index) => (record[index] ?? '').trim()

  return records
    .filter((record) => record.some((value) => (value ?? '').trim() !== ''))
    .map((record, index) => {
      return {
        id: `${keyPrefix}-${index + 1}`,
        note: getAt(record, 0),
        done: getAt(record, 1).toUpperCase() === 'TRUE',
        content: getAt(record, 2),
        task: getAt(record, 4),
        cost: getAt(record, 5),
        rewardRes: getAt(record, 6),
        rewardXp: getAt(record, 7),
        cpProd: getAt(record, 8),
        pop: getAt(record, 9),
        balance: getAt(record, 10),
        exp: getAt(record, 11),
        cp: getAt(record, 12),
        popTotal: getAt(record, 13),
      }
    })
}

const TRAILING_ROW_COUNT = 17

function splitChecklistRows(rows) {
  if (rows.length <= TRAILING_ROW_COUNT) {
    return {
      main: rows,
      trailing: [],
    }
  }

  return {
    main: rows.slice(0, -TRAILING_ROW_COUNT),
    trailing: rows.slice(-TRAILING_ROW_COUNT),
  }
}

const fourPartySimRows = mapChecklistRows(fourPartySimCsv, '4p-sim')
const fourPartyFarmRows = mapChecklistRows(fourPartyFarmCsv, '4p-farm')
const threePartySimRows = mapChecklistRows(threePartySimCsv, '3p-sim')
const threePartyFarmRows = mapChecklistRows(threePartyFarmCsv, '3p-farm')

const splitFourPartySim = splitChecklistRows(fourPartySimRows)
const splitFourPartyFarm = splitChecklistRows(fourPartyFarmRows)
const splitThreePartySim = splitChecklistRows(threePartySimRows)
const splitThreePartyFarm = splitChecklistRows(threePartyFarmRows)

export const CHECKLIST_DATA = {
  '4 Party - Sim': splitFourPartySim.main,
  '4 Party - Farm': splitFourPartyFarm.main,
  '3 Party - Sim': splitThreePartySim.main,
  '3 Party - Farm': splitThreePartyFarm.main,
}

export const COMMON_TRAILING_DATA = splitFourPartySim.trailing
