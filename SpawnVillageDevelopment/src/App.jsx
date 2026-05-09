import { Fragment, useEffect, useMemo, useState } from 'react'
import { additionCpBuildingNote } from './data/addition-cp-building-note'
import { settler_cost, stable_change, troop_cost, warehouse_change } from './data/troop'
import PWABadge from './PWABadge.jsx'
import { TABS } from './tabs'
import './App.css'

const INITIALS = {
  balance: 3000,
  exp: 0,
  cp: 0,
  population: 0,
}

const QUANTITY_ROW_INDEX = 68
const QUANTITY_ROW_UNIT_COST = 1635
const QUANTITY_FEATURE_TAB_ID = '4p-sim'
const APP_SETTINGS_STORAGE_KEY = 'spawn-checklist:settings:v1'

function loadStoredSettings() {
  if (typeof window === 'undefined') {
    return null
  }

  const legacyChecks = {}

  TABS.forEach((tab) => {
    try {
      const saved = window.localStorage.getItem(`spawn-checklist:${tab.id}`)
      if (!saved) {
        legacyChecks[tab.id] = {}
        return
      }

      const parsed = JSON.parse(saved)
      legacyChecks[tab.id] = parsed && typeof parsed === 'object' ? parsed : {}
    } catch {
      legacyChecks[tab.id] = {}
    }
  })

  try {
    const raw = window.localStorage.getItem(APP_SETTINGS_STORAGE_KEY)
    if (!raw) {
      return { checkedByTab: legacyChecks }
    }

    const parsed = JSON.parse(raw)
    if (!parsed || typeof parsed !== 'object') {
      return { checkedByTab: legacyChecks }
    }

    return {
      ...parsed,
      checkedByTab:
        parsed.checkedByTab && typeof parsed.checkedByTab === 'object'
          ? parsed.checkedByTab
          : legacyChecks,
    }
  } catch {
    return { checkedByTab: legacyChecks }
  }
}

function getNumber(value) {
  return Number(value) || 0
}

function formatNumber(value) {
  return getNumber(value).toLocaleString()
}

function formatRounded(value) {
  return Math.round(getNumber(value)).toLocaleString()
}

function buildComputedRows(rows, options = {}) {
  const {
    calcInsertIndex = -1,
    secondCalcInsertIndex = -1,
    researchCost = 0,
    trainCost = 0,
    secondTrainCost = 0,
    enableCalcRows = false,
    applyEquitesOverrides = false,
    stableOverrideIndex = -1,
    warehouseUpdateIndex = -1,
    selectedSettlerCost = 0,
    quantityValue = 5,
    enableQuantityFormula = false,
    beforeCpInsertIndex = -1,
    afterCpInsertIndex = -1,
    enableCpBuildingInsertion = false,
    beforeCpRows = [],
    afterCpRows = [],
  } = options
  let balance = INITIALS.balance
  let previousRewardRes = 0
  let exp = INITIALS.exp
  let cp = INITIALS.cp
  let population = INITIALS.population

  const applyComputedValues = (rowData) => {
    balance += previousRewardRes - getNumber(rowData.cost)
    const displayBalance = balance

    previousRewardRes = getNumber(rowData['reward res'])
    exp += getNumber(rowData['reward exp'])
    cp += getNumber(rowData['cp prod'])
    population += getNumber(rowData.pop)

    return {
      ...rowData,
      balance: displayBalance,
      exp,
      cp,
      population,
    }
  }

  const result = []

  rows.forEach((row, index) => {
    let currentRow = row

    if (String(row['To do']).toLowerCase().includes('train settler')) {
      currentRow = {
        ...row,
        cost: getNumber(row.task) * selectedSettlerCost,
      }
    }

    if (enableQuantityFormula && index === QUANTITY_ROW_INDEX) {
      currentRow = {
        ...currentRow,
        cost: getNumber(quantityValue) * QUANTITY_ROW_UNIT_COST,
      }
    }

    if (enableCpBuildingInsertion && index === beforeCpInsertIndex) {
      result.push({ kind: 'cp-control-before' })

      beforeCpRows.forEach((cpRow, cpRowIndex) => {
        result.push(
          applyComputedValues({
            kind: 'cp-additional-before',
            checkKey: `cp-before-${cpRow.sourceIndex}`,
            displayNo: '',
            canRemove: cpRowIndex === beforeCpRows.length - 1,
            ...cpRow,
          }),
        )
      })
    }

    if (applyEquitesOverrides && index === stableOverrideIndex) {
      currentRow = {
        ...row,
        task: stable_change[1].task,
        cost: stable_change[1].cost,
        'cp prod': stable_change[1]['cp prod'],
        pop: stable_change[1].pop,
      }
    }

    if (applyEquitesOverrides && index === warehouseUpdateIndex) {
      currentRow = {
        ...row,
        task: warehouse_change[1].task,
        cost: warehouse_change[1].cost,
        'cp prod': warehouse_change[1]['cp prod'],
        pop: warehouse_change[1].pop,
      }
    }

    if (enableCalcRows && index === calcInsertIndex) {
      if (applyEquitesOverrides) {
        result.push(
          applyComputedValues({
            kind: 'task',
            checkKey: 'equites-warehouse-to-5',
            displayNo: '',
            'To do': 'Warehouse',
            Tier: 0,
            task: warehouse_change[0].task,
            cost: warehouse_change[0].cost,
            'reward res': 0,
            'reward exp': 0,
            'cp prod': warehouse_change[0]['cp prod'],
            pop: warehouse_change[0].pop,
          }),
        )
      }

      result.push(
        applyComputedValues({
          kind: 'calc-research',
          cost: researchCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )

      result.push(
        applyComputedValues({
          kind: 'calc-train',
          cost: trainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    if (enableCalcRows && index === secondCalcInsertIndex) {
      result.push(
        applyComputedValues({
          kind: 'calc-train-2',
          cost: secondTrainCost,
          'reward res': 0,
          'reward exp': 0,
          'cp prod': 0,
          pop: 0,
        }),
      )
    }

    result.push(
      applyComputedValues({
        kind: 'task',
        checkKey: String(index),
        index,
        ...currentRow,
      }),
    )

    if (enableCpBuildingInsertion && index === afterCpInsertIndex) {
      result.push({ kind: 'cp-control-after' })

      afterCpRows.forEach((cpRow, cpRowIndex) => {
        result.push(
          applyComputedValues({
            kind: 'cp-additional-after',
            checkKey: `cp-after-${cpRow.sourceIndex}`,
            displayNo: '',
            canRemove: cpRowIndex === afterCpRows.length - 1,
            ...cpRow,
          }),
        )
      })
    }
  })

  return result
}

function App() {
  const storedSettings = useMemo(() => loadStoredSettings(), [])
  const [activeTab, setActiveTab] = useState(() => {
    const savedTab = storedSettings?.activeTab
    return TABS.some((tab) => tab.id === savedTab) ? savedTab : TABS[0].id
  })
  const [checkedByTab, setCheckedByTab] = useState(() => {
    if (storedSettings?.checkedByTab && typeof storedSettings.checkedByTab === 'object') {
      return storedSettings.checkedByTab
    }
    return {}
  })
  const [cpPlacementByTab, setCpPlacementByTab] = useState(() => {
    return storedSettings?.cpPlacementByTab && typeof storedSettings.cpPlacementByTab === 'object'
      ? storedSettings.cpPlacementByTab
      : {}
  })
  const [quantityByTab, setQuantityByTab] = useState(() => {
    return storedSettings?.quantityByTab && typeof storedSettings.quantityByTab === 'object'
      ? storedSettings.quantityByTab
      : {}
  })
  const [selectedTribe, setSelectedTribe] = useState(() => {
    const savedTribe = storedSettings?.selectedTribe
    return settler_cost.some((tribe) => tribe.unit === savedTribe)
      ? savedTribe
      : (settler_cost[0]?.unit ?? '')
  })
  const [selectedUnit, setSelectedUnit] = useState(() => {
    const savedUnit = storedSettings?.selectedUnit
    return troop_cost.some((troop) => troop.unit === savedUnit)
      ? savedUnit
      : (troop_cost[0]?.unit ?? '')
  })
  const [farmUnitCount, setFarmUnitCount] = useState(() => {
    return typeof storedSettings?.farmUnitCount === 'number'
      ? getNumber(storedSettings.farmUnitCount)
      : 10
  })
  const [farmUnitCountSecond, setFarmUnitCountSecond] = useState(() => {
    return typeof storedSettings?.farmUnitCountSecond === 'number'
      ? getNumber(storedSettings.farmUnitCountSecond)
      : 10
  })

  useEffect(() => {
    const nextSettings = {
      activeTab,
      checkedByTab,
      cpPlacementByTab,
      quantityByTab,
      selectedTribe,
      selectedUnit,
      farmUnitCount,
      farmUnitCountSecond,
    }

    window.localStorage.setItem(APP_SETTINGS_STORAGE_KEY, JSON.stringify(nextSettings))
  }, [
    activeTab,
    checkedByTab,
    cpPlacementByTab,
    quantityByTab,
    selectedTribe,
    selectedUnit,
    farmUnitCount,
    farmUnitCountSecond,
  ])

  const activeTabConfig = TABS.find((tab) => tab.id === activeTab) ?? TABS[0]
  const notesByIndex = useMemo(() => {
    const grouped = {}

    ;(activeTabConfig.notes ?? []).forEach((noteItem) => {
      const noteIndex = Number(noteItem.index)
      if (Number.isNaN(noteIndex) || !noteItem.note) {
        return
      }

      if (!grouped[noteIndex]) {
        grouped[noteIndex] = []
      }

      grouped[noteIndex].push(noteItem.note)
    })

    return grouped
  }, [activeTabConfig])

  const activeChecks = checkedByTab[activeTab] ?? {}
  const activeQuantity = quantityByTab[activeTab] ?? 1
  const selectedSettler = useMemo(
    () => settler_cost.find((tribe) => tribe.unit === selectedTribe) ?? settler_cost[0],
    [selectedTribe],
  )
  const selectedTroop = useMemo(
    () => troop_cost.find((troop) => troop.unit === selectedUnit) ?? troop_cost[0],
    [selectedUnit],
  )
  const activeFarmConfig = activeTabConfig.farmConfig
  const activeCpConfig = activeTabConfig.cpBuildingConfig
  const activeAdditionalRows = activeTabConfig.additionRows ?? []
  const activeCpPlacement = cpPlacementByTab[activeTab] ?? { beforeCount: 0, afterCount: 0 }
  const beforeCount = activeCpPlacement.beforeCount ?? 0
  const afterCount = activeCpPlacement.afterCount ?? 0
  const beforeCpRows = activeAdditionalRows
    .slice(0, beforeCount)
    .map((row, index) => ({ ...row, sourceIndex: index }))
  const afterCpRows = activeAdditionalRows
    .slice(beforeCount, beforeCount + afterCount)
    .map((row, index) => ({ ...row, sourceIndex: beforeCount + index }))
  const remainingCpRows = activeAdditionalRows.slice(beforeCount + afterCount)
  const selectedSettlerCost = getNumber(selectedSettler?.cost)

  const researchCost = getNumber(selectedTroop?.research)
  const trainCost = getNumber(farmUnitCount) * getNumber(selectedTroop?.train)
  const secondTrainCost = getNumber(farmUnitCountSecond) * getNumber(selectedTroop?.train)
  const breakEven = researchCost + trainCost
  const secondBreakEven = secondTrainCost
  const fullRaids = getNumber(selectedTroop?.capacity)
    ? breakEven / getNumber(selectedTroop.capacity)
    : 0
  const secondFullRaids = getNumber(selectedTroop?.capacity)
    ? secondBreakEven / getNumber(selectedTroop.capacity)
    : 0
  const perHour24 = breakEven / 24
  const perHour48 = breakEven / 48
  const secondPerHour24 = secondBreakEven / 24
  const secondPerHour48 = secondBreakEven / 48
  const enableQuantityFeature = activeTab === QUANTITY_FEATURE_TAB_ID
  const showFarmCalc = Boolean(activeFarmConfig)
  const applyEquitesOverrides = showFarmCalc && selectedTroop?.unit === 'Equites Imperatoris'
  const enableCpBuildingInsertion = Boolean(activeCpConfig)
  const beforeCpInsertIndex = activeCpConfig?.beforeInsertIndex ?? activeCpConfig?.insertIndex ?? -1
  const afterCpInsertIndex = activeCpConfig?.afterInsertIndex ?? activeCpConfig?.insertIndex ?? -1
  const computedRows = useMemo(
    () =>
      buildComputedRows(activeTabConfig.rows, {
        calcInsertIndex: activeFarmConfig?.firstInsertIndex ?? -1,
        secondCalcInsertIndex: activeFarmConfig?.secondInsertIndex ?? -1,
        researchCost,
        trainCost,
        secondTrainCost,
        enableCalcRows: showFarmCalc,
        applyEquitesOverrides,
        stableOverrideIndex: activeFarmConfig?.equitesStableIndex ?? -1,
        warehouseUpdateIndex: activeFarmConfig?.equitesWarehouseUpdateIndex ?? -1,
        selectedSettlerCost,
        quantityValue: activeQuantity,
        enableQuantityFormula: enableQuantityFeature,
        beforeCpInsertIndex,
        afterCpInsertIndex,
        enableCpBuildingInsertion,
        beforeCpRows,
        afterCpRows,
      }),
    [
      activeTabConfig,
      activeFarmConfig,
      researchCost,
      trainCost,
      secondTrainCost,
      showFarmCalc,
      applyEquitesOverrides,
      selectedSettlerCost,
      activeQuantity,
      enableQuantityFeature,
      beforeCpInsertIndex,
      afterCpInsertIndex,
      enableCpBuildingInsertion,
      beforeCpRows,
      afterCpRows,
    ],
  )

  const addCpBuilding = (position) => {
    setCpPlacementByTab((previous) => {
      const current = previous[activeTab] ?? { beforeCount: 0, afterCount: 0 }
      const usedCount = current.beforeCount + current.afterCount

      if (usedCount >= activeAdditionalRows.length) {
        return previous
      }

      const nextPlacement = position === 'before'
        ? {
            beforeCount: current.beforeCount + 1,
            afterCount: current.afterCount,
          }
        : {
            beforeCount: current.beforeCount,
            afterCount: current.afterCount + 1,
          }

      return {
        ...previous,
        [activeTab]: nextPlacement,
      }
    })
  }

  const removeCpBuilding = (position) => {
    setCpPlacementByTab((previous) => {
      const current = previous[activeTab] ?? { beforeCount: 0, afterCount: 0 }

      if (position === 'before') {
        if (current.beforeCount <= 0) {
          return previous
        }

        return {
          ...previous,
          [activeTab]: {
            beforeCount: current.beforeCount - 1,
            afterCount: current.afterCount,
          },
        }
      }

      if (position === 'after') {
        if (current.afterCount <= 0) {
          return previous
        }

        return {
          ...previous,
          [activeTab]: {
            beforeCount: current.beforeCount,
            afterCount: current.afterCount - 1,
          },
        }
      }

      return previous
    })
  }

  const toggleDone = (rowKey) => {
    setCheckedByTab((previous) => {
      const currentTabChecks = previous[activeTab] ?? {}
      const nextTabChecks = {
        ...currentTabChecks,
        [rowKey]: !currentTabChecks[rowKey],
      }

      const next = {
        ...previous,
        [activeTab]: nextTabChecks,
      }

      window.localStorage.setItem(
        `spawn-checklist:${activeTab}`,
        JSON.stringify(nextTabChecks),
      )

      return next
    })
  }

  function handleResetSettings() {
    window.localStorage.removeItem(APP_SETTINGS_STORAGE_KEY)

    TABS.forEach((tab) => {
      window.localStorage.removeItem(`spawn-checklist:${tab.id}`)
    })

    window.location.reload()
  }

  useEffect(() => {
    if (activeTabConfig && activeTabConfig.label) {
      document.title = `${activeTabConfig.label} | Spawn Village Development Guide`;
    } else {
      document.title = 'Spawn Village Development Guide';
    }
  }, [activeTabConfig]);

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Spawn Village Development Guide</h1>
        <h3>Credit to Caim's guide <a href="https://docs.google.com/spreadsheets/d/1A2ku0fdzpJDOefjG8ryUyyxYbXD2_z76IxMUhSERjtE/edit?usp=sharing" target="_blank" rel="noopener noreferrer">here</a></h3>
        <p>We have auto-save feature, you can close the browser and come back later without losing your progress.</p>
        <div className="header-controls">
          <button type="button" className="reset-btn" onClick={handleResetSettings}>
            Reset All Settings
          </button>
          <div className="tribe-select-wrap">
            <label htmlFor="tribe-select">Tribe:</label>
            <select
              id="tribe-select"
              value={selectedTribe}
              onChange={(event) => setSelectedTribe(event.target.value)}
            >
              {settler_cost.map((tribe) => (
                <option key={tribe.unit} value={tribe.unit}>
                  {tribe.unit}
                </option>
              ))}
            </select>
          </div>
        </div>
      </header>

      <nav className="tabs" aria-label="Checklist tabs">
        {TABS.map((tab) => (
          <button
            key={tab.id}
            type="button"
            className={`tab-btn ${tab.id === activeTab ? 'active' : ''}`}
            onClick={() => setActiveTab(tab.id)}
          >
            {tab.label}
          </button>
        ))}
      </nav>

      <section className="table-wrap">
        <table>
          <thead>
            <tr>
              <th>No.</th>
              <th>Done</th>
              <th>To do</th>
              <th>Task</th>
              <th>cost</th>
              <th>reward res</th>
              <th>reward exp</th>
              <th>cp prod</th>
              <th>pop</th>
              <th>balance</th>
              <th>exp</th>
              <th>cp</th>
              <th>population</th>
            </tr>
          </thead>
          <tbody>
            {computedRows.map((row) => {
              if (row.kind === 'cp-control-before') {
                return (
                  <tr key={`${activeTab}-cp-control-before`} className="cp-control-row">
                    <td></td>
                    <td></td>
                    <td colSpan={11}>
                      <button
                        type="button"
                        className="cp-btn"
                        onClick={() => addCpBuilding('before')}
                        disabled={!remainingCpRows.length}
                      >
                        Add CP building
                      </button>
                    </td>
                  </tr>
                )
              }

              if (row.kind === 'cp-control-after') {
                return (
                  <tr key={`${activeTab}-cp-control-after`} className="cp-control-row">
                    <td></td>
                    <td></td>
                    <td colSpan={11}>
                      <button
                        type="button"
                        className="cp-btn"
                        onClick={() => addCpBuilding('after')}
                        disabled={!remainingCpRows.length}
                      >
                        Add CP building
                      </button>
                    </td>
                  </tr>
                )
              }

              if (row.kind === 'calc-research') {
                return (
                  <tr key={`${activeTab}-calc-research`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Research</td>
                    <td>
                      <select
                        value={selectedUnit}
                        onChange={(event) => setSelectedUnit(event.target.value)}
                        className="farm-calc-input farm-calc-unit-input"
                        aria-label="Select troop unit"
                      >
                        {troop_cost.map((troop) => (
                          <option key={troop.unit} value={troop.unit}>
                            {troop.unit}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td className="farm-calc-value">{formatNumber(researchCost)}</td>
                    <td className="farm-calc-label">Break even</td>
                    <td className="farm-calc-label">Full raids</td>
                    <td className="farm-calc-label">Per hour (24h)</td>
                    <td className="farm-calc-label">Per hour (48h)</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train') {
                return (
                  <tr key={`${activeTab}-calc-train`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCount}
                          onChange={(event) => setFarmUnitCount(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(trainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(breakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(fullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(perHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              if (row.kind === 'calc-train-2') {
                return (
                  <tr key={`${activeTab}-calc-train-2`} className="farm-calc-row">
                    <td></td>
                    <td></td>
                    <td className="farm-calc-label">Farm units</td>
                    <td>
                      <div className="farm-calc-control">
                        <input
                          type="number"
                          min="0"
                          value={farmUnitCountSecond}
                          onChange={(event) => setFarmUnitCountSecond(getNumber(event.target.value))}
                          className="farm-calc-input"
                          aria-label="Farm units to make second"
                        />
                        <span className="farm-calc-suffix">to make</span>
                      </div>
                    </td>
                    <td className="farm-calc-value">{formatNumber(secondTrainCost)}</td>
                    <td className="farm-calc-value">{formatNumber(secondBreakEven)}</td>
                    <td className="farm-calc-value">{formatRounded(secondFullRaids)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour24)}</td>
                    <td className="farm-calc-value">{formatRounded(secondPerHour48)}</td>
                    <td className="farm-calc-value">{formatNumber(row.balance)}</td>
                    <td className="farm-calc-value">{formatNumber(row.exp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.cp)}</td>
                    <td className="farm-calc-value">{formatNumber(row.population)}</td>
                  </tr>
                )
              }

              const rowCheckKey = row.checkKey ?? String(row.index)
              const isAdditionalCpRow = row.kind === 'cp-additional-before' || row.kind === 'cp-additional-after'
              const isDone = Boolean(activeChecks[rowCheckKey])
              const rowNotes = notesByIndex[row.index] ?? []
              const rowNumberLabel = row.displayNo ?? row.index + 1
              const rowKeyBase = row.index ?? rowCheckKey
              const isQuantityRow =
                enableQuantityFeature && row.kind === 'task' && row.index === QUANTITY_ROW_INDEX

              return (
                <Fragment key={`${activeTab}-${rowKeyBase}-group`}>
                  {rowNotes.map((note, noteIndex) => (
                    <tr key={`${activeTab}-${rowKeyBase}-note-${noteIndex}`} className="note-row">
                      <td colSpan={13}>{note}</td>
                    </tr>
                  ))}
                  <tr key={`${activeTab}-${rowKeyBase}`} className={isDone ? 'done-row' : ''}>
                    <td>{rowNumberLabel}</td>
                    <td>
                      {isAdditionalCpRow ? (
                        <button
                          type="button"
                          className="cp-btn remove"
                          disabled={!row.canRemove}
                          onClick={() => removeCpBuilding(row.kind === 'cp-additional-before' ? 'before' : 'after')}
                        >
                          Remove
                        </button>
                      ) : (
                        <input
                          type="checkbox"
                          checked={isDone}
                          onChange={() => toggleDone(rowCheckKey)}
                          aria-label={`Mark row ${rowNumberLabel || rowCheckKey} as done`}
                        />
                      )}
                    </td>
                    <td>{row['To do']}</td>
                    <td>{row.task}</td>
                    <td>{formatNumber(row.cost)}</td>
                    {isQuantityRow ? (
                      <>
                        <td className="quantity-label-cell">Quantity:</td>
                        <td>
                          <input
                            type="number"
                            min="0"
                            value={activeQuantity}
                            onChange={(event) => {
                              const nextValue = getNumber(event.target.value)
                              setQuantityByTab((previous) => ({
                                ...previous,
                                [activeTab]: nextValue,
                              }))
                            }}
                            className="quantity-input"
                            aria-label="Crannies quantity"
                          />
                        </td>
                      </>
                    ) : (
                      <>
                        <td>{formatNumber(row['reward res'])}</td>
                        <td>{formatNumber(row['reward exp'])}</td>
                      </>
                    )}
                    <td>{row['cp prod']}</td>
                    <td>{row.pop}</td>
                    <td>{formatNumber(row.balance)}</td>
                    <td>{row.exp}</td>
                    <td>{row.cp}</td>
                    <td>{row.population}</td>
                  </tr>
                </Fragment>
              )
            })}
          </tbody>
        </table>
      </section>

      <section className="cp-additional-section">
        <h2>Additional CP buildings</h2>
        {additionCpBuildingNote.map((note, index) => (
          <p key={`cp-additional-note-${index}`}>{note}</p>
        ))}
        <div className="table-wrap">
          <table>
            <thead>
              <tr>
                <th>To do</th>
                <th>Task</th>
                <th>cost</th>
                <th>reward res</th>
                <th>reward exp</th>
                <th>cp prod</th>
                <th>pop</th>
              </tr>
            </thead>
            <tbody>
              {remainingCpRows.length ? (
                remainingCpRows.map((row, index) => (
                  <tr key={`remaining-cp-${index}`}>
                    <td>{row['To do']}</td>
                    <td>{row.task}</td>
                    <td>{formatNumber(row.cost)}</td>
                    <td>{formatNumber(row['reward res'])}</td>
                    <td>{formatNumber(row['reward exp'])}</td>
                    <td>{row['cp prod']}</td>
                    <td>{row.pop}</td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={7}>No remaining CP buildings.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </section>

      <div className="pwa-badge-wrap">
        <PWABadge />
      </div>
    </main>
  )
}

export default App
