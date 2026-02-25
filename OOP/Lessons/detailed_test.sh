#!/bin/bash

SERVER="localhost"
PORT=4004

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

test_passed=0
test_failed=0

send_command() {
    local cmd="$1"
    local desc="$2"
    local expected="$3"

    echo -e "${BLUE}╔════════════════════════════════════════════════════════════════${NC}"
    echo -e "${BLUE}║ Test: $desc${NC}"
    echo -e "${BLUE}╠════════════════════════════════════════════════════════════════${NC}"
    echo -e "${BLUE}║ Command:${NC} $cmd"
    echo -e "${BLUE}╠════════════════════════════════════════════════════════════════${NC}"

    local response=$(
        exec 3<>/dev/tcp/$SERVER/$PORT 2>&1
        echo "$cmd" >&3
        timeout 2 cat <&3
        exec 3>&-
    )

    echo -e "${BLUE}║ Response:${NC}"
    if [ -z "$response" ]; then
        echo -e "${YELLOW}║   (empty response)${NC}"
    else
        echo "$response" | while IFS= read -r line; do
            echo -e "${BLUE}║${NC}   $line"
        done
    fi

    if [ ! -z "$expected" ]; then
        if echo "$response" | grep -q "$expected"; then
            echo -e "${BLUE}║${NC}"
            echo -e "${BLUE}║ Status:${NC} ${GREEN}✓ PASSED${NC}"
            ((test_passed++))
        else
            echo -e "${BLUE}║${NC}"
            echo -e "${BLUE}║ Status:${NC} ${RED}✗ FAILED${NC} (expected: $expected)"
            ((test_failed++))
        fi
    else
        echo -e "${BLUE}║${NC}"
        echo -e "${BLUE}║ Status:${NC} ${GREEN}✓ EXECUTED${NC}"
        ((test_passed++))
    fi

    echo -e "${BLUE}╚════════════════════════════════════════════════════════════════${NC}"
    echo ""

    sleep 0.5
}

echo -e "${YELLOW}╔══════════════════════════════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}║         SERVER COMMAND TESTING - DETAILED DEBUG                  ║${NC}"
echo -e "${YELLOW}╚══════════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Test 1: Help command
send_command "help" "Help command - should return help text" ""

# Test 2: Get all meals
send_command "get_meals" "Get all meals - should return JSON array" "Борщ"

# Test 3: Get all orders
send_command "get_orders" "Get all orders - should return JSON array" "StudentID"

# Test 4: Get meal by valid ID
send_command 'get_meal_by_id {"id": "Суп1"}' "Get meal by valid ID (Суп1)" "Борщ"

# Test 5: Get meal by invalid ID
send_command 'get_meal_by_id {"id": "INVALID_ID"}' "Get meal by invalid ID" "404"

# Test 6: Get order by valid ID
send_command 'get_order_by_id {"id": "i00s0001"}' "Get order by valid ID (i00s0001)" "i00s0001"

# Test 7: Get order by invalid ID
send_command 'get_order_by_id {"id": "INVALID_ID"}' "Get order by invalid ID" "404"

# Test 8: Add new meal
send_command 'add_meal {"id": "TEST_MEAL_999", "title": "Тестовое блюдо", "cost": 999}' "Add new meal" "successfully"

# Test 9: Verify meal was added
send_command 'get_meal_by_id {"id": "TEST_MEAL_999"}' "Verify added meal exists" "Тестовое блюдо"

# Test 10: Add order with complete data
send_command 'add_order {"StudentID": "TEST_ORDER_999", "date": "2026-02-09", "meals": [{"id": "Суп1", "title": "Борщ", "cost": 100}]}' "Add order with date and meals" "successfully"

# Test 11: Verify order was added and check date
send_command 'get_order_by_id {"id": "TEST_ORDER_999"}' "Verify added order and date field" "TEST_ORDER_999"

# Test 12: Delete test meal
send_command 'delete_meal {"id": "TEST_MEAL_999"}' "Delete test meal" "successfully"

# Test 13: Verify meal was deleted
send_command 'get_meal_by_id {"id": "TEST_MEAL_999"}' "Verify meal was deleted (should get 404)" "404"

# Test 14: Delete test order
send_command 'delete_order {"id": "TEST_ORDER_999"}' "Delete test order" "successfully"

# Test 15: Verify order was deleted
send_command 'get_order_by_id {"id": "TEST_ORDER_999"}' "Verify order was deleted (should get 404)" "404"

# Test 16: Try to add meal with duplicate ID
send_command 'add_meal {"id": "Суп1", "title": "Duplicate", "cost": 50}' "Try to add meal with duplicate ID" ""

# Test 17: Try to delete non-existent meal
send_command 'delete_meal {"id": "NON_EXISTENT"}' "Try to delete non-existent meal" ""

# Test 18: Exit command
send_command "exit" "Exit command - should close connection" ""

echo -e "${YELLOW}╔══════════════════════════════════════════════════════════════════╗${NC}"
echo -e "${YELLOW}║                      TEST SUMMARY                                ║${NC}"
echo -e "${YELLOW}╠══════════════════════════════════════════════════════════════════╣${NC}"
echo -e "${YELLOW}║${NC} ${GREEN}Passed:${NC} $test_passed tests                                              "
echo -e "${YELLOW}║${NC} ${RED}Failed:${NC} $test_failed tests                                              "
echo -e "${YELLOW}╚══════════════════════════════════════════════════════════════════╝${NC}"
